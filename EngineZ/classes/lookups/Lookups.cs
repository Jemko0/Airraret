using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EngineZ.DataStructures;
using EngineZ.classes.world;
using System.Collections.Generic;

namespace EngineZ.ID
{
    public static class EntityID
    {
        public static EntityDef? GetEntity(EEntityTypes type)
        {
            EntityDef e = new EntityDef();
            e.type = type;
            e.tint = Color.White;

            switch (type)
            {
                case EEntityTypes.None:
                    break;

                case EEntityTypes.Default:
                    e.dimensions = new Vector2(20, 40);
                    e.sprite = Main.GetGame().Content.Load<Texture2D>("textures/rock");
                    return e;

                case EEntityTypes.Player:
                    e.dimensions = new Vector2(20, 40);
                    e.sprite = Main.GetGame().Content.Load<Texture2D>("textures/rock");
                    e.tint = Color.Red;
                    return e;
            }

            return null;
        }
    }

    public static class TileID
    {
        public static Tile GetTile(ETileTypes type)
        {
            Tile t = new Tile();

            //defaults
            t.framePadding = 2;
            t.frameSize = 16;
            t.valid = true;
            t.collide = true;
            t.tint = Color.White;
            t.light = 0x00;
            t.blockLight = 0x03;
            t.useTileFrame = true;
            t.hideWall = true;
            t.frameIgnoreTop = false;

            switch (type)
            {
                case ETileTypes.Air:
                    t.valid = false;
                    break;

                case ETileTypes.Dirt:
                    t.sprite = Main.GetGame().Content.Load<Texture2D>("textures/tile/dirt");
                    break;

                case ETileTypes.Grass:
                    t.sprite = Main.GetGame().Content.Load<Texture2D>("textures/tile/grass");
                    break;

                case ETileTypes.Torch:
                    t.sprite = Main.GetGame().Content.Load<Texture2D>("textures/tile/torch");
                    t.blockLight = 0;
                    t.light = 16;
                    t.hideWall = false;
                    t.frameIgnoreTop = true;
                    break;
            }

            return t;
        }
    }

    public static class WallID
    {
        public static Wall GetWall(EWallTypes type)
        {
            Wall wall = new Wall();
            wall.framePadding = 2;
            wall.frameSize = 16;
            wall.tint = Color.White;
            switch(type)
            {
                case EWallTypes.Air:
                    break;

                case EWallTypes.Dirt:
                    wall.sprite = Main.GetGame().Content.Load<Texture2D>("textures/wall/dirt");
                    break;
            }

            return wall;
        }
    }
}
