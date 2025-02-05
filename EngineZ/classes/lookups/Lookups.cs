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

            switch (type)
            {
                case EntityTypes.None:
                    break;

                case EntityTypes.Default:
                    e.dimensions = new Vector2(20, 40);
                    e.sprite = Main.GetGame().Content.Load<Texture2D>("textures/rock");
                    return e;
            }

            return null;
       }
    }
}
