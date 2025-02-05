using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineZ.DataStructures;
using EngineZ.ID;

namespace EngineZ.Entities
{
    public class Entity : GameComponent
    {
        public EntityDef entity;
        public Vector2 position;
        public Vector2 velocity = new Vector2(1, 0);
        public Color tint = Color.White;
        public Entity(Game game, EntityTypes initType) : base(game)
        {
            entity.type = initType;
            Enabled = true;
            Initialize();
        }

        public override void Initialize()
        {
            base.Initialize();
            entity = (EntityDef)EntityID.GetEntity(entity.type);
        }

        public override void Update(GameTime gameTime)
        {
            position += velocity;
            base.Update(gameTime);
        }
    }
}
