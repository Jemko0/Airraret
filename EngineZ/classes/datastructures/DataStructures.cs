using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

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
        Dirt,
        Stone,
        Grass,
    }

    public enum EWallTypes
    {
        Air,
        Dirt,
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
        public int frameSize;
        public int framePadding;
        public Color tint;
    }

    public struct Wall
    {
        public Texture2D sprite;
        public int frameSize;
        public int framePadding;
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

        public IntVector2(Vector2 v)
        {
            this.X = (int)v.X;
            this.Y = (int)v.Y;
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

    public enum ELightingMethod
    {
        Basic = 0,

    }
}
