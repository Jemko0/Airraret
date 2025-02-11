using EngineZ.DataStructures;
using EngineZ.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.classes.world
{
    public static class Lighting
    {
        public const int MAX_LIGHT = 16;
        public static int OutdoorLight = 16;
        public const int MIN_LIGHT = 0;
        public static void UpdateLighting(Vector2 position)
        {
            if (!World.IsTileOrWall(position))
            {
                World.lightMap[position] = OutdoorLight;
                return;
            }

            Tile tileData = new Tile();
            if (World.tiles.ContainsKey(position))
            {
                tileData = TileID.GetTile(World.tiles[position]);
            }

            if (tileData.light > MIN_LIGHT)
            {
                World.lightMap[position] = tileData.light;
                return;
            }

            int brightestNeighbor = GetBrightestNeighborLight(position);
            int blockValue = (World.IsValidTile(position)? tileData.blockLight : 1);

            World.lightMap[position] = brightestNeighbor - blockValue;
        }

        public static int GetBrightestNeighborLight(Vector2 middle)
        {
            int[] neighborLight = new int[]
            {
                GetLightLevel(middle + new Vector2(World.TILESIZE, 0)), //R
                GetLightLevel(middle + new Vector2(-World.TILESIZE, 0)), //L
                GetLightLevel(middle + new Vector2(0, World.TILESIZE)), //B
                GetLightLevel(middle + new Vector2(0, -World.TILESIZE)), //T
            };

            int brightestNeighborValue = -1;

            foreach(int light in neighborLight)
            {
                if(light > brightestNeighborValue)
                {
                    brightestNeighborValue = light;
                }
            }

            return brightestNeighborValue;
        }

        public static void AddLight(Vector2 pos, int light)
        {
            World.lightMap[pos] = light;
        }

        public static int GetLightLevel(Vector2 position)
        {
            if (World.lightMap.ContainsKey(position))
            {
                if (World.IsTileOrWall(position))
                {
                    return World.lightMap[position];
                }
                else
                {
                    return OutdoorLight;
                }
            }

            return OutdoorLight;
        }
    }
}
