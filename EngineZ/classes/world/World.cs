using EngineZ.DataStructures;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
        public static int TILESIZE = 32;

        public static List<WorldGenTask> tasks = new List<WorldGenTask>();
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
            tasks.Add(new WGT_Terrain("fill"));
            tasks.Add(new WGT_FindSpawn("spawn"));
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
            for (int x = 0; x < wparams.maxTilesX; x++)
            {
                int surfaceY = wparams.maxTilesY - (int)(Math.Sin(x * 0.1) * 24);
                World.worldSurfaceTiles.Add(surfaceY);

                for (int y = -surfaceY; y < wparams.maxTilesY; y++)
                {
                    
                    World.tiles.Add(new Vector2(x * World.TILESIZE, y * World.TILESIZE), ETileTypes.Dirt);
                }
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

            World.worldSpawn = new Vector2(sX * 32, -(sY + 2) * 32);

            World.CompleteCurrent();
        }
    }
}
