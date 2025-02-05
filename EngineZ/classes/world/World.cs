using EngineZ.DataStructures;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace EngineZ.classes.world
{
    public class World
    {
        public static Dictionary<Vector2, TileTypes> tiles = new Dictionary<Vector2, TileTypes>();
        public static int TILESIZE = 32;
        public void FillWorld(int size)
        {
            for(int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    tiles.Add(new Vector2(x * TILESIZE, y * TILESIZE), TileTypes.Dirt);
                }
            }
        }

        public void SetTile(Vector2 tile, TileTypes type)
        {
            tiles[tile] = type;
        }
    }
}
