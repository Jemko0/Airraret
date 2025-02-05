using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineZ.DataStructures;
using EngineZ.ID;
using EngineZ.classes.interfaces;

namespace EngineZ.Entities
{
    public class Entity : GameComponent, IPlayerControllerInput
    {
        public EntityDef type;
        public Rectangle rect;
        public Vector2 velocity = new Vector2(0, 0);
        public Entity(Game game, EntityTypes initType) : base(game)
        {
            type.type = initType;
            Enabled = true;
            Initialize();
        }

        public virtual void AxisInput(float axisVal)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// gets the entities rect collider within the world
        /// </summary>
        /// <returns></returns>
        public Rectangle GetRect()
        {
            return rect;
        }

        public override void Initialize()
        {
            base.Initialize();
            type = (EntityDef)EntityID.GetEntity(type.type);
            rect.Width = (int)type.dimensions.X;
            rect.Height = (int)type.dimensions.Y;
        }

        public void SetLocation(Vector2 position)
        {
            rect.Location = new Point((int)position.X, (int)position.Y);
        }

        public void SetLocation(float x, float y)
        {
            rect.Location = new Point((int)x, (int)y);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
