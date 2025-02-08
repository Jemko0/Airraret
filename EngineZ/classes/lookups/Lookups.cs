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

            switch (type)
            {
                case ETileTypes.Air:
                    t.valid = false;
                    break;

                case ETileTypes.Dirt:
                    t.sprite = Main.GetGame().Content.Load<Texture2D>("textures/dirt");
                    break;

                case ETileTypes.Grass:
                    t.sprite = Main.GetGame().Content.Load<Texture2D>("textures/grass");
                    break;
            }

            return t;
        }
    }
}
