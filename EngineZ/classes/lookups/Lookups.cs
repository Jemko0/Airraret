using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EngineZ.DataStructures;

namespace EngineZ.ID
{
    public static class EntityID
    {
        public static EntityDef? GetEntity(EntityTypes type)
        {
            EntityDef e = new EntityDef();
            e.type = type;
            e.tint = Color.White;

            switch (type)
            {
                case EntityTypes.None:
                    break;

                case EntityTypes.Default:
                    e.dimensions = new Vector2(20, 40);
                    e.sprite = Main.GetGame().Content.Load<Texture2D>("textures/rock");
                    return e;

                case EntityTypes.Player:
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
        public static Tile GetTile(TileTypes type)
        {
            Tile t = new Tile();

            switch (type)
            {
                case TileTypes.Air:
                    t.valid = false;
                    break;

                case TileTypes.Dirt:
                    t.valid = true;
                    t.collide = true;
                    t.sprite = Main.GetGame().Content.Load<Texture2D>("textures/dirt");
                    t.tint = Color.White;
                    break;
            }

            return t;
        }
    }
}
