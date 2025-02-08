using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.DataStructures
{
    public enum EEntityTypes
    {
        None = -1,
        Default,
        Player,
    }

    public enum ETileTypes
    {
        Air = 0,
        Dirt = 1,
        Stone = 2,
    }

    public struct EntityDef
    {
        public EEntityTypes type;
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

    public enum ETextJustification
    {
        Left,
        Center,
        Right,
    }
    public enum ELogCategory
    {
        LogUndefined = 0,
        LogUI,
        LogEntity,
        LogPlayerController,
        LogWorldGen,
    }

    public enum EWidgetAlignment
    {
        TopLeft,
        Fill,
    }

    public struct IntVector2 : IEqualityComparer<IntVector2>
    {
        public int X;
        public int Y;
        public IntVector2(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public bool Equals(IntVector2 x, IntVector2 y)
        {
            return x.X == y.X && x.Y == y.Y;
        }

        public int GetHashCode([DisallowNull] IntVector2 obj)
        {
            return (X.GetHashCode() * 397) ^ Y.GetHashCode();
        }
    }
}
