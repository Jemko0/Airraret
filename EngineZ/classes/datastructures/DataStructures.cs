using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.DataStructures
{
    public enum EntityTypes
    {
        None = -1,
        Default,
        Player,
    }

    public enum TileTypes
    {
        Air = 0,
        Dirt = 1,
        Stone = 2,
    }

    public struct EntityDef
    {
        public EntityTypes type;
        public Vector2 dimensions;
        public Texture2D sprite;
        public Color tint;
    }

    public struct Tile
    {
        public bool valid;
        public bool collide;
        public Texture2D sprite;
        public Color tint;
    }
}
