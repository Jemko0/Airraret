using EngineZ.DataStructures;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        public static int TILESIZE = 16;

        public static List<WorldGenTask> tasks = new List<WorldGenTask>();
        public static Dictionary<Vector2, int> lightMap = new Dictionary<Vector2, int>();
        public static Dictionary<Vector2, Rectangle> tileFrames = new Dictionary<Vector2, Rectangle>();
        public Progress<WorldGenProgress> currentTaskProgress;
        private static TaskCompletionSource<bool> currentCompletionSource;
        
        public delegate void TaskProgressChanged(object sender, WorldGenProgress e);
        public event TaskProgressChanged taskProgressChanged;
        public static List<float?> worldSurfaceTiles = new List<float?>();
        public static Vector2 worldSpawn;
        public static Dictionary<Vector2, int> lightSources = new Dictionary<Vector2, int>();

        //Initialize Tasks
        public void GenFillWorld()
        {
            tasks.Add(new WGT_Reset("reset"));
            tasks.Add(new WGT_Terrain("fill"));
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
            bool b1 = IsValidTile(xWorld + World.TILESIZE, yWorld); //RIGHT
            bool b2 = IsValidTile(xWorld - World.TILESIZE, yWorld); //LEFT
            bool b3 = IsValidTile(xWorld, yWorld + World.TILESIZE); //TOP
            bool b4 = IsValidTile(xWorld, yWorld - World.TILESIZE); //BOTTOM

            return b1 || b2 || b3 || b4;
        }

        public static void SetTile(int xWorld, int yWorld, ETileTypes type)
        {
            Vector2 k = new Vector2(xWorld, yWorld);
            if (tiles.ContainsKey(k))
            {
                tiles[k] = type;
            }
        }

        public static Rectangle GetTileFrame(int xWorld, int yWorld, Tile tileData)
        {
            Rectangle tileFrame = new Rectangle(0, 0, tileData.frameSize, tileData.frameSize);

            bool r = IsValidTile(xWorld + World.TILESIZE, yWorld); //RIGHT
            bool l = IsValidTile(xWorld - World.TILESIZE, yWorld); //LEFT
            bool t = IsValidTile(xWorld, yWorld - World.TILESIZE); //TOP
            bool b = IsValidTile(xWorld, yWorld + World.TILESIZE); //BOTTOM

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

            Rectangle frameR = GetTileFrame(neighborLocations[0].X, neighborLocations[0].Y, tileData);
            Rectangle frameL = GetTileFrame(neighborLocations[1].X, neighborLocations[1].Y, tileData);
            Rectangle frameT = GetTileFrame(neighborLocations[2].X, neighborLocations[2].Y, tileData);
            Rectangle frameB = GetTileFrame(neighborLocations[3].X, neighborLocations[3].Y, tileData);

            tileFrames[new Vector2(neighborLocations[0].X, neighborLocations[0].Y)] = frameR;
            tileFrames[new Vector2(neighborLocations[1].X, neighborLocations[1].Y)] = frameL;
            tileFrames[new Vector2(neighborLocations[2].X, neighborLocations[2].Y)] = frameT;
            tileFrames[new Vector2(neighborLocations[3].X, neighborLocations[3].Y)] = frameB;
        }

        public static bool IsValidTile(int xWorld, int yWorld)
        {
            if(tiles.ContainsKey(new Vector2(xWorld, yWorld)))
            {
                return tiles[new Vector2(xWorld, yWorld)] != ETileTypes.Air;
            }
            return false;
        }

        public static void UpdateLighting(Vector2 position, int lightLevel)
        {
            // Don't process if light level is 0 or if the new light level would be lower
            if (lightLevel <= 0 || (lightMap.ContainsKey(position) && lightMap[position] >= lightLevel))
            {
                return;
            }

            // Update current tile's light level
            lightMap[position] = lightLevel;
            
            // Spread light to neighboring tiles with decreased intensity
            Vector2[] neighbors = new Vector2[]
            {
                new Vector2(position.X + TILESIZE, position.Y),     // Right
                new Vector2(position.X - TILESIZE, position.Y),     // Left
                new Vector2(position.X, position.Y + TILESIZE),     // Down
                new Vector2(position.X, position.Y - TILESIZE),     // Up
            };

            // Calculate new light level - less reduction when there are more solid neighbors
            int newLight = lightLevel - 1;
            
            // Propagate to neighbors
            foreach (Vector2 neighbor in neighbors)
            {
                if (tiles.ContainsKey(neighbor))
                {
                    // Air propagates light better than solid blocks
                    int neighborNewLight = tiles[neighbor] == ETileTypes.Air || !tiles.ContainsKey(neighbor) ? newLight : newLight - 1;
                    UpdateLighting(neighbor, neighborNewLight);
                }
            }
        }

        public static int GetLightLevel(Vector2 position)
        {
            if (lightMap.ContainsKey(position))
            {
                return lightMap[position];
            }
            return 0;
        }

        public static void RecalculateLightingAroundPoint(Vector2 position, int radius = 32)
        {
            HashSet<Vector2> lightSourcePositions = new HashSet<Vector2>();

            // First, clear lighting in the affected area
            for (int x = -radius; x <= radius; x += TILESIZE)
            {
                for (int y = -radius; y <= radius; y += TILESIZE)
                {
                    Vector2 checkPos = new Vector2(position.X + x, position.Y + y);
                    if (lightMap.ContainsKey(checkPos))
                    {
                        lightMap.Remove(checkPos);
                    }

                    // If this position is air or doesn't exist (void), it's a natural light source
                    if (!tiles.ContainsKey(checkPos) || tiles[checkPos] == ETileTypes.Air)
                    {
                        lightSourcePositions.Add(checkPos);
                    }
                    // If it's a solid tile but adjacent to air, add the adjacent air positions as light sources
                    else
                    {
                        Vector2[] neighbors = new Vector2[]
                        {
                            new Vector2(checkPos.X + TILESIZE, checkPos.Y),  // Right
                            new Vector2(checkPos.X - TILESIZE, checkPos.Y),  // Left
                            new Vector2(checkPos.X, checkPos.Y + TILESIZE),  // Down
                            new Vector2(checkPos.X, checkPos.Y - TILESIZE)   // Up
                        };

                        foreach (Vector2 neighbor in neighbors)
                        {
                            if (!tiles.ContainsKey(neighbor) || tiles[neighbor] == ETileTypes.Air)
                            {
                                lightSourcePositions.Add(neighbor);
                            }
                        }
                    }
                }
            }

            // Apply natural lighting from air/void (always intensity 16)
            foreach (Vector2 source in lightSourcePositions)
            {
                UpdateLighting(source, 16);
            }

            // Apply artificial lighting from light sources within radius
            foreach (var lightSource in lightSources)
            {
                if (Vector2.Distance(lightSource.Key, position) <= radius)
                {
                    UpdateLighting(lightSource.Key, lightSource.Value);
                }
            }
        }

        public static void AddLightSource(Vector2 position, int intensity)
        {
            lightSources[position] = intensity;
            RecalculateLightingAroundPoint(position);
        }

        public static void RemoveLightSource(Vector2 position)
        {
            if (lightSources.ContainsKey(position))
            {
                lightSources.Remove(position);
                RecalculateLightingAroundPoint(position);
            }
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
            int caveSeed = (int)(wparams.seed + (5415.0325 / 23 * 1.42) + (wparams.seed - 523));
            for (int x = 0; x < wparams.maxTilesX; x++)
            {
                int surfaceTileY = CalcSurface(x, wparams, noise);
                
                World.worldSurfaceTiles.Add(surfaceTileY);
                ETileTypes tileType = ETileTypes.Air;

                for (int y = surfaceTileY; y > 0; y--)
                {
                    noise.SetSeed(caveSeed);
                    noise.SetFrequency(0.031f);
                    noise.SetCellularReturnType(FastNoiseLite.CellularReturnType.Distance2Add);
                    noise.SetFractalGain(3.7425f);
                    noise.SetFractalOctaves(1);
                    float caveVal = noise.GetNoise(x, y);
                    if(caveVal > -0.25)
                    {
                        tileType = PickType(x, y, wparams);
                        World.tiles.Add(new Vector2(x * World.TILESIZE, -y * World.TILESIZE), tileType);
                    }
                    
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
            noise.SetSeed(wparams.seed);
            noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(4);
            noise.SetFractalLacunarity(1.819654321f);
            noise.SetFrequency(0.01245f);

            float noise1 = (noise.GetNoise(x, 0)) * 5.11f;

            noise.SetSeed((int)(wparams.seed + 215 * 0.52134));
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFractalOctaves(3);
            noise.SetFractalLacunarity(1.5f);
            noise.SetFrequency(0.01134f);

            float noise2 = (noise.GetNoise(x, 0)) * 8f;


            float finalVal = noise1 * noise2;
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
            World.lightMap.Clear();

            // First pass: Set initial light values
            foreach (var tile in World.tiles)
            {
                Vector2 pos = tile.Key;
                int tileX = (int)(pos.X / World.TILESIZE);
                float surfaceHeight = (float)World.worldSurfaceTiles[tileX];
                float tileY = -pos.Y / World.TILESIZE;  // Convert to positive Y for comparison

                // If it's air or above surface, it's a light source
                if (tile.Value == ETileTypes.Air || tileY >= surfaceHeight)
                {
                    World.lightMap[pos] = 16;
                }
                else
                {
                    World.lightMap[pos] = 0;
                }
            }

            // Second pass: Propagate light
            foreach (var tile in World.tiles)
            {
                Vector2 pos = tile.Key;
                if (World.lightMap[pos] == 16)
                {
                    // Use existing UpdateLighting method for light propagation
                    World.UpdateLighting(pos, 16);
                }
            }

            World.CompleteCurrent();
        }
    }
}
