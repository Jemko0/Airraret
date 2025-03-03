﻿using EngineZ.DataStructures;
using EngineZ.ID;
using EngineZ.RNGenerator;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;

namespace EngineZ.classes.world
{
    public class WorldGenParams
    {
        public int seed { get; set; }
        public int maxTilesX { get; set; }
        public int maxTilesY { get; set; }
    }
    public class WorldGenProgress
    {
        public float PercentComplete { get; set; }
        public string CurrentTask { get; set; }
        public bool isCompleted { get; set; }
    }

    public class World
    {
        public static Dictionary<Vector2, ETileTypes> tiles = new Dictionary<Vector2, ETileTypes>();
        public static Dictionary<Vector2, EWallTypes> walls = new Dictionary<Vector2, EWallTypes>();

        public const int TILESIZE = 24;

        public static List<WorldGenTask> tasks = new List<WorldGenTask>();
        public static Dictionary<Vector2, int> lightMap = new Dictionary<Vector2, int>();
        public static Dictionary<Vector2, Rectangle> tileFrames = new Dictionary<Vector2, Rectangle>();
        public static Dictionary<Vector2, Rectangle> wallFrames = new Dictionary<Vector2, Rectangle>();
        public Progress<WorldGenProgress> currentTaskProgress;
        private static TaskCompletionSource<bool> currentCompletionSource;
        
        public delegate void TaskProgressChanged(object sender, WorldGenProgress e);
        public event TaskProgressChanged taskProgressChanged;
        public static List<float?> worldSurfaceTiles = new List<float?>();
        public static Vector2 worldSpawn;

        //Initialize Tasks
        public void GenFillWorld()
        {
            tasks.Add(new WGT_Reset("reset"));
            tasks.Add(new WGT_Terrain("terrain"));
            tasks.Add(new WGT_Caves("caves"));
            tasks.Add(new WGT_FindSpawn("spawn"));
            tasks.Add(new WGT_FinalizeLightTiles("light"));
        }

        public async Task GenerateWorld(WorldGenParams newParams)
        {
            foreach (var task in tasks)
            {
                currentCompletionSource = new TaskCompletionSource<bool>();
                currentTaskProgress = new Progress<WorldGenProgress>();
                currentTaskProgress.ProgressChanged += CurrentTaskProgressChanged;
            
                // Start Task and await both Task and CompletionSource
                var runTask = task.Run(currentTaskProgress, newParams);
                await Task.WhenAll(runTask, currentCompletionSource.Task);
            
                Logger.Log(ELogCategory.LogWorldGen, "WGENTask: " + task.taskName + "completed");
            }

            Logger.Log("WorldGenFinished");
        }

        public static void CompleteCurrent()
        {
            if (currentCompletionSource != null && !currentCompletionSource.Task.IsCompleted)
            {
                currentCompletionSource.SetResult(true);
            }
        }

        private void CurrentTaskProgressChanged(object sender, WorldGenProgress e)
        {
            taskProgressChanged?.Invoke(this, e);
        }

        public static void CreateWorld()
        {
            Main.GetGame().world = new World();
        }

        public static float GetSurfaceHeightAtIdx(int xIndex)
        {
            return (float)worldSurfaceTiles[xIndex];
        }

        public static bool IsTileExposedToAir(int xWorld, int yWorld)
        {
            bool b1 = !IsTileOrWall(xWorld + World.TILESIZE, yWorld); //RIGHT
            bool b2 = !IsTileOrWall(xWorld - World.TILESIZE, yWorld); //LEFT
            bool b3 = !IsTileOrWall(xWorld, yWorld + World.TILESIZE); //TOP
            bool b4 = !IsTileOrWall(xWorld, yWorld - World.TILESIZE); //BOTTOM

            return b1 || b2 || b3 || b4;
        }

        public static bool IsTileExposedToAir(Vector2 pos)
        {
            return IsTileExposedToAir((int)pos.X, (int)pos.Y);
        }

        public static void SetTile(int xWorld, int yWorld, ETileTypes type, bool updateFrames = false)
        {
            Vector2 k = new Vector2(xWorld, yWorld);
            if (tiles.ContainsKey(k))
            {
                tiles[k] = type;
            }
            else
            {
                tiles.Add(k, type);
            }

            //UpdateLighting(k, type == ETileTypes.Air ? 16 : 0);
            if(updateFrames)
            {
                
                UpdateTileFramesAt((int)k.X, (int)k.Y, ID.TileID.GetTile(type));
            }
        }

        public static void SetWall(int xWorld, int yWorld, EWallTypes type)
        {
            Vector2 k = new Vector2(xWorld, yWorld);

            if (walls.ContainsKey(k))
            {
                walls[k] = type;
            }
            else
            {
                walls.Add(k, type);
            }

            //UpdateLighting(k, 0);
            UpdateWallFramesAt((int)k.X, (int)k.Y, ID.WallID.GetWall(type));
        }

        public static void CreateHole(int xWorld, int yWorld, int size)
        {
            size += 1;
            int x = -size;
            int y = 0;

            while(x < size)
            {
                while (y < size - Math.Abs(x))
                {
                    if (tiles.ContainsKey(new Vector2(xWorld + x * TILESIZE, yWorld + y * TILESIZE)))
                    {
                        SetWall(xWorld + x * TILESIZE, yWorld + y * TILESIZE, EWallTypes.Dirt);
                        SetWall(xWorld + x * TILESIZE, yWorld - y * TILESIZE, EWallTypes.Dirt);
                    }

                    if (!(x == size - 1 - y || x == -size + 1 + y))
                    {
                        SetTile(xWorld + x * TILESIZE, yWorld + y * TILESIZE, ETileTypes.Air);
                        SetTile(xWorld + x * TILESIZE, yWorld - y * TILESIZE, ETileTypes.Air);
                    }
                    y++;
                }
                x++;
                y = 0;
            }
        }

        public static void DigTunnel(int xWorld, int yWorld, int size, int steps, int dirX, int dirY)
        {
            for (int step = 0; step < steps; step++)
            {
                int posX = xWorld + (step * dirX);
                int posY = yWorld + (step * dirY);


                posX = (int)MathUtil.FloatToTileSnap(posX);
                posY = (int)MathUtil.FloatToTileSnap(posY);

                CreateHole(posX, posY, size);
            }
        }

        #region Frames
        public static Rectangle GetTileFrame(int xWorld, int yWorld, Tile tileData)
        {
            if(tileData.hangsOnWalls)
            {
                return GetHangingTileFrame(xWorld, yWorld, tileData);
            }

            Rectangle tileFrame = new Rectangle(0, 0, tileData.frameSize, tileData.frameSize);

            bool r = IsValidForTileFrame(xWorld + World.TILESIZE, yWorld); //RIGHT
            bool l = IsValidForTileFrame(xWorld - World.TILESIZE, yWorld); //LEFT
            bool t = IsValidForTileFrame(xWorld, yWorld - World.TILESIZE); //TOP
            bool b = IsValidForTileFrame(xWorld, yWorld + World.TILESIZE); //BOTTOM

            int frameSlot = tileData.frameSize + tileData.framePadding;

            if (r && !l && !t && !b)
            {
                tileFrame.X = frameSlot;
                tileFrame.Y = 0;
            }

            if (!r && l && !t && !b)
            {
                tileFrame.X = frameSlot * 2;
                tileFrame.Y = 0;
            }

            if (!r && !l && t && !b)
            {
                tileFrame.X = frameSlot * 3;
                tileFrame.Y = 0;
            }

            if (!r && !l && !t && b)
            {
                tileFrame.X = frameSlot * 4;
                tileFrame.Y = 0;
            }

            if(r && l && t && b)
            {
                tileFrame.X = frameSlot * 5;
                tileFrame.Y = 0;
            }

            
            //counts for R and L
            if (r && l && !t && !b)
            {
                tileFrame.X = 0;
                tileFrame.Y = frameSlot;
            }

            #region RIGHT + OTHER
            if (r && !l && t && !b) //RT
            {
                tileFrame.X = frameSlot;
                tileFrame.Y = frameSlot;
            }

            if (r && !l && !t && b) //RB
            {
                tileFrame.X = frameSlot * 2;
                tileFrame.Y = frameSlot;
            }

            if (r && !l && t && b) //RTB
            {
                tileFrame.X = 0;
                tileFrame.Y = frameSlot * 2;
            }
            #endregion

            #region LEFT + OTHER
            if (!r && l && t && !b) //LT
            {
                tileFrame.X = frameSlot * 3;
                tileFrame.Y = frameSlot;
            }

            if (!r && l && !t && b) //LB
            {
                tileFrame.X = frameSlot * 4;
                tileFrame.Y = frameSlot;
            }

            if (!r && l && t && b) //LTB
            {
                tileFrame.X = frameSlot;
                tileFrame.Y = frameSlot * 2;
            }
            #endregion

            if (r && l && !t && b) //RLB
            {
                tileFrame.X = frameSlot * 2;
                tileFrame.Y = frameSlot * 2;
            }

            if (r && l && t && !b) //RLT
            {
                tileFrame.X = frameSlot * 3;
                tileFrame.Y = frameSlot * 2;
            }

            if (!r && !l && !t && !b) //0
            {
                tileFrame.X = frameSlot * 4;
                tileFrame.Y = frameSlot * 2;
            }

            if(!r && !l && t && b)
            {
                tileFrame.X = frameSlot * 5;
                tileFrame.Y = frameSlot * 1;
            }

            return tileFrame;
        }


        public static Rectangle GetHangingTileFrame(int xWorld, int yWorld, Tile tileData)
        {
            Rectangle tileFrame = new Rectangle(0, 0, tileData.frameSize, tileData.frameSize);

            bool r = IsValidForTileFrame(xWorld + World.TILESIZE, yWorld); //RIGHT
            bool l = IsValidForTileFrame(xWorld - World.TILESIZE, yWorld); //LEFT
            bool b = IsValidForTileFrame(xWorld, yWorld + World.TILESIZE); //BOTTOM

            int frameSlot = tileData.frameSize + tileData.framePadding;

            if (r)
            {
                tileFrame.X = frameSlot * 2;
                tileFrame.Y = frameSlot * 0;
                return tileFrame;
            }
            if (l)
            {
                tileFrame.X = frameSlot * 1;
                tileFrame.Y = frameSlot * 0;
                return tileFrame;
            }
            if (b)
            {
                tileFrame.X = frameSlot * 0;
                tileFrame.Y = frameSlot * 0;
                return tileFrame;
            }

            return tileFrame;
        }

        public static Rectangle GetWallFrame(int xWorld, int yWorld, Wall wallData)
        {
            Rectangle wallFrame = new Rectangle(0, 0, wallData.frameSize, wallData.frameSize);

            bool r = IsValidWall(xWorld + World.TILESIZE, yWorld); //RIGHT
            bool l = IsValidWall(xWorld - World.TILESIZE, yWorld); //LEFT
            bool t = IsValidWall(xWorld, yWorld - World.TILESIZE); //TOP
            bool b = IsValidWall(xWorld, yWorld + World.TILESIZE); //BOTTOM

            int frameSlot = wallData.frameSize + wallData.framePadding;

            if (r && !l && !t && !b)
            {
                wallFrame.X = frameSlot;
                wallFrame.Y = 0;
            }

            if (!r && l && !t && !b)
            {
                wallFrame.X = frameSlot * 2;
                wallFrame.Y = 0;
            }

            if (!r && !l && t && !b)
            {
                wallFrame.X = frameSlot * 3;
                wallFrame.Y = 0;
            }

            if (!r && !l && !t && b)
            {
                wallFrame.X = frameSlot * 4;
                wallFrame.Y = 0;
            }

            if (r && l && t && b)
            {
                wallFrame.X = frameSlot * 5;
                wallFrame.Y = 0;
            }


            //counts for R and L
            if (r && l && !t && !b)
            {
                wallFrame.X = 0;
                wallFrame.Y = frameSlot;
            }

            #region RIGHT + OTHER
            if (r && !l && t && !b) //RT
            {
                wallFrame.X = frameSlot;
                wallFrame.Y = frameSlot;
            }

            if (r && !l && !t && b) //RB
            {
                wallFrame.X = frameSlot * 2;
                wallFrame.Y = frameSlot;
            }

            if (r && !l && t && b) //RTB
            {
                wallFrame.X = 0;
                wallFrame.Y = frameSlot * 2;
            }
            #endregion

            #region LEFT + OTHER
            if (!r && l && t && !b) //LT
            {
                wallFrame.X = frameSlot * 3;
                wallFrame.Y = frameSlot;
            }

            if (!r && l && !t && b) //LB
            {
                wallFrame.X = frameSlot * 4;
                wallFrame.Y = frameSlot;
            }

            if (!r && l && t && b) //LTB
            {
                wallFrame.X = frameSlot;
                wallFrame.Y = frameSlot * 2;
            }
            #endregion

            if (r && l && !t && b) //RLB
            {
                wallFrame.X = frameSlot * 2;
                wallFrame.Y = frameSlot * 2;
            }

            if (r && l && t && !b) //RLT
            {
                wallFrame.X = frameSlot * 3;
                wallFrame.Y = frameSlot * 2;
            }

            if (!r && !l && !t && !b) //0
            {
                wallFrame.X = frameSlot * 4;
                wallFrame.Y = frameSlot * 2;
            }

            if (!r && !l && t && b)
            {
                wallFrame.X = frameSlot * 5;
                wallFrame.Y = frameSlot * 1;
            }

            return wallFrame;
        }

        public static void UpdateTileFramesAt(int xWorld, int yWorld, Tile tileData)
        {
            //TileData = data after the update already happened meaning if tile destroyed -> air
            IntVector2[] neighborLocations = new IntVector2[]
            {
                new IntVector2(xWorld + TILESIZE, yWorld),
                new IntVector2(xWorld - TILESIZE, yWorld),
                new IntVector2(xWorld, yWorld + TILESIZE),
                new IntVector2(xWorld, yWorld - TILESIZE),
            };

            tileFrames[new Vector2(xWorld, yWorld)] = GetTileFrame(xWorld, yWorld, tileData);

            Tile[] tileDatas = new Tile[4];
            
            for (int i = 0; i < tileDatas.Length; i++)
            {
                Vector2 pos = new Vector2(neighborLocations[i].X, neighborLocations[i].Y);
                if (tiles.ContainsKey(pos))
                {
                    tileDatas[i] = TileID.GetTile(tiles[pos]);
                }
                else
                {
                    tileDatas[i] = TileID.GetTile(ETileTypes.Air);
                }
            }
                

            Rectangle frameR = GetTileFrame(neighborLocations[0].X, neighborLocations[0].Y, tileDatas[0]);
            Rectangle frameL = GetTileFrame(neighborLocations[1].X, neighborLocations[1].Y, tileDatas[1]);
            Rectangle frameT = GetTileFrame(neighborLocations[2].X, neighborLocations[2].Y, tileDatas[2]);
            Rectangle frameB = GetTileFrame(neighborLocations[3].X, neighborLocations[3].Y, tileDatas[3]);

            tileFrames[new Vector2(neighborLocations[0].X, neighborLocations[0].Y)] = frameR;
            tileFrames[new Vector2(neighborLocations[1].X, neighborLocations[1].Y)] = frameL;
            tileFrames[new Vector2(neighborLocations[2].X, neighborLocations[2].Y)] = frameT;
            tileFrames[new Vector2(neighborLocations[3].X, neighborLocations[3].Y)] = frameB;
        }

        public static void UpdateWallFramesAt(int xWorld, int yWorld, Wall wallData)
        {
            //TileData = data after the update already happened meaning if tile destroyed -> air
            IntVector2[] neighborLocations = new IntVector2[]
            {
                new IntVector2(xWorld + TILESIZE, yWorld),
                new IntVector2(xWorld - TILESIZE, yWorld),
                new IntVector2(xWorld, yWorld + TILESIZE),
                new IntVector2(xWorld, yWorld - TILESIZE),
            };

            Rectangle frameR = GetWallFrame(neighborLocations[0].X, neighborLocations[0].Y, wallData);
            Rectangle frameL = GetWallFrame(neighborLocations[1].X, neighborLocations[1].Y, wallData);
            Rectangle frameT = GetWallFrame(neighborLocations[2].X, neighborLocations[2].Y, wallData);
            Rectangle frameB = GetWallFrame(neighborLocations[3].X, neighborLocations[3].Y, wallData);

            wallFrames[new Vector2(neighborLocations[0].X, neighborLocations[0].Y)] = frameR;
            wallFrames[new Vector2(neighborLocations[1].X, neighborLocations[1].Y)] = frameL;
            wallFrames[new Vector2(neighborLocations[2].X, neighborLocations[2].Y)] = frameT;
            wallFrames[new Vector2(neighborLocations[3].X, neighborLocations[3].Y)] = frameB;
        }
        #endregion
        public static bool IsValidTile(int xWorld, int yWorld)
        {
            if(tiles.ContainsKey(new Vector2(xWorld, yWorld)))
            {
                return tiles[new Vector2(xWorld, yWorld)] != ETileTypes.Air;
            }
            return false;
        }

        public static bool IsValidForTileFrame(int xWorld, int yWorld)
        {
            if (IsValidTile(xWorld, yWorld))
            {
                return TileID.GetTile(tiles[new Vector2(xWorld, yWorld)]).affectTileFrames;   
            }
            return false;
        }

        public static bool IsValidTile(Vector2 pos)
        {
            return IsValidTile((int)pos.X, (int)pos.Y);
        }

        public static bool IsValidWall(int xWorld, int yWorld)
        {
            if (walls.ContainsKey(new Vector2(xWorld, yWorld)))
            {
                return walls[new Vector2(xWorld, yWorld)] != EWallTypes.Air;
            }
            return false;
        }

        public static bool IsValidWall(Vector2 pos)
        {
            return IsValidWall((int)pos.X, (int)pos.Y);
        }

        public static bool IsTileOrWall(Vector2 pos)
        {
            return IsValidTile(pos) || IsValidWall(pos);
        }

        public static bool IsTileOrWall(int xW, int yW)
        {
            return IsValidTile(new Vector2(xW, yW)) || IsValidWall(new Vector2(xW, yW));
        }
    }


    /// <summary>
    /// A World gen Task runs async to generate the world while being able to report progress
    /// </summary>
    public class WorldGenTask
    {
        public string taskName { get; set; }
        public WorldGenTask(string identifier)
        {
            taskName = identifier;
        }

        public virtual void InitTask()
        {
            
        }

        public virtual async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {

        }
    }

    public class WGT_Reset : WorldGenTask
    {
        public WGT_Reset(string identifier) : base(identifier)
        {
        }

        public override async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {
            World.worldSurfaceTiles.Clear();
            World.tiles.Clear();
            World.worldSpawn = new Vector2(0, 0);
            World.tileFrames.Clear();
            World.lightMap.Clear();

            progress?.Report(new WorldGenProgress()
            {
                CurrentTask = "Reset",
                PercentComplete = 1.0f,
            });

            await Task.Delay(500);
            World.CompleteCurrent();
        }
    }

    public class WGT_FillWorld : WorldGenTask
    {
        public WGT_FillWorld(string identifier) : base(identifier)
        {
        }

        public override async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {
            for (int x = 0; x < wparams.maxTilesX; x++)
            {
                for (int y = 0; y < wparams.maxTilesY; y++)
                {
                    if (y == 0)
                    {
                        World.worldSurfaceTiles.Add(y * World.TILESIZE);
                    }

                    World.tiles.Add(new Vector2(x * World.TILESIZE, y * World.TILESIZE), ETileTypes.Dirt);

                    progress?.Report(new WorldGenProgress()
                    {
                        CurrentTask = "Filling World...",
                        PercentComplete = ((float)x / wparams.maxTilesX),
                    });
                }
            }
            World.CompleteCurrent();
        }
    }

    public class WGT_Terrain : WorldGenTask
    {
        public WGT_Terrain(string identifier) : base(identifier)
        {
        }

        public override async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {
            int surfaceLevel = wparams.maxTilesY / 3;
            FastNoiseLite noise = new FastNoiseLite();

            for (int x = 0; x < wparams.maxTilesX; x++)
            {
                int surfaceTileY = CalcSurface(x, wparams, noise);
                
                World.worldSurfaceTiles.Add(surfaceTileY);

                ETileTypes tileType = ETileTypes.Air;

                for (int y = surfaceTileY; y > 0; y--)
                {
                    tileType = PickType(x, y, wparams);
                    World.tiles.Add(new Vector2(x * World.TILESIZE, -y * World.TILESIZE), tileType);
                    World.walls.Add(new Vector2(x * World.TILESIZE, -y * World.TILESIZE), EWallTypes.Dirt);
                }

                progress?.Report(new WorldGenProgress()
                {
                    CurrentTask = "Terrain",
                    PercentComplete = ((float)x / wparams.maxTilesX),
                });
            }
            World.CompleteCurrent();
        }

        public int CalcSurface(int x, WorldGenParams wparams, FastNoiseLite noise)
        {
            //noise1
            noise.SetSeed(wparams.seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(4);
            noise.SetFractalLacunarity(1.819654321f);
            noise.SetFrequency(0.01245f);
            
            float noise1 = (noise.GetNoise(x, 0)) * 5.11f;
            //------

            //noise2
            noise.SetSeed((int)(wparams.seed + 215 * 0.52134));
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(3);
            noise.SetFractalLacunarity(1.5f);
            noise.SetFrequency(0.04134f);

            float noise2 = (noise.GetNoise(x, 0)) * 8f;
            //------

            //noise3
            noise.SetSeed((int)(wparams.seed + 1111 * 3.241));
            noise.SetFractalType(FastNoiseLite.FractalType.Ridged);
            noise.SetFractalOctaves(2);
            noise.SetFractalLacunarity(1.1f);
            noise.SetFrequency(0.005f);

            float noise3 = (noise.GetNoise(x, 0)) * 24;
            //------

            //noise3mult
            noise.SetSeed((int)(wparams.seed - 324 * 5.241));
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(2);
            noise.SetFractalLacunarity(1.3f);
            noise.SetFrequency(0.0085f);

            float noise3mult = (noise.GetNoise(x, 0)) * 3f;
            //------

            float finalVal = (noise1 * noise2) + (noise3 * noise3mult);
            return wparams.maxTilesY + (int)finalVal;
        }

        public ETileTypes PickType(int x, int y, WorldGenParams wparams)
        {
            if(y == World.worldSurfaceTiles[x])
            {
                return ETileTypes.Grass;
            }
            return ETileTypes.Dirt;
        }
    }

    public class WGT_Caves : WorldGenTask
    {
        public WGT_Caves(string identifier) : base(identifier)
        {
        }

        public override async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {
            int caveAmount = wparams.maxTilesX + wparams.maxTilesY / 5;

            const int tunnelMaxStep = 12;
            const int caveMaxSize = 9;

            for (int i = 0; i < caveAmount; i++)
            {
                int seedOffset = (int)(RNG.GetPseudoRandomFloat(16 - wparams.seed + i * 32.01f) * ushort.MaxValue);
                float xPercentage = RNG.GetPseudoRandomFloat(wparams.seed + seedOffset + i);
                
                seedOffset = (int)(RNG.GetPseudoRandomFloat(wparams.seed * 0.3f - 1412 - i * 15.3f) * ushort.MaxValue);
                float yPercentage = RNG.GetPseudoRandomFloat(wparams.seed + seedOffset + i);


                int caveX = (int)(wparams.maxTilesX * xPercentage);
                int caveY = (int)(wparams.maxTilesY * yPercentage);
                
                caveX *= World.TILESIZE;
                caveY *= World.TILESIZE;

                int caveSize = (int)(RNG.GetPseudoRandomFloat(wparams.seed - i * 11.04f) * caveMaxSize);
                int tunnelSteps = (int)(RNG.GetPseudoRandomFloat(wparams.seed + i * 3.034f) * tunnelMaxStep);

                int dirX = (int)((RNG.GetPseudoRandomFloat(wparams.seed - i * 13.42314f) - 0.5f) * 256);
                int dirY = (int)((RNG.GetPseudoRandomFloat(wparams.seed + i * 9.8671f)     - 0.5f) * 256);

                Logger.Log("direction ", dirX, dirY);

                World.DigTunnel(caveX, -caveY, caveSize, tunnelSteps, dirX, dirY);

                

                progress?.Report(new WorldGenProgress()
                {
                    CurrentTask = "Carving Holes...",
                    PercentComplete = (float)i / caveAmount,
                });

            }

            World.CompleteCurrent();
        }
    }

    public class WGT_FindSpawn : WorldGenTask
    {
        public WGT_FindSpawn(string identifier) : base(identifier)
        {
        }

        public override async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {
            int sX = wparams.maxTilesX / 2;
            float sY = World.GetSurfaceHeightAtIdx(sX);

            World.worldSpawn = new Vector2(sX * World.TILESIZE, -(sY + 4) * World.TILESIZE);

            progress?.Report(new WorldGenProgress()
            {
                CurrentTask = "Finding Spawn",
                PercentComplete = 1.0f,
            });

            World.CompleteCurrent();
        }
    }

    public class WGT_FinalizeLightTiles : WorldGenTask
    {
        public WGT_FinalizeLightTiles(string identifier) : base(identifier)
        {
        }

        public override async Task Run(IProgress<WorldGenProgress> progress, WorldGenParams wparams)
        {
            foreach(var tile in World.tiles)
            {
                Lighting.UpdateLighting(tile.Key);

                progress?.Report(new WorldGenProgress()
                {
                    CurrentTask = "Lighting Tiles...",
                    PercentComplete = (tile.Key.X / World.TILESIZE) / wparams.maxTilesX,
                });
            }
            
            World.CompleteCurrent();
        }
    }
}
