using EngineZ.classes.cam;
using EngineZ.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace EngineZ.classes.gameplay
{
    public class PlayerController : GameComponent
    {
        public int id;
        public string name;
        private Entity? controlledEntity;
        public bool cameraAttachToEntity = false;
        public PlayerController(Game game, int id) : base(game)
        {
            this.id = id;
            Main.GetGame().Components.Add(this);
        }

        public void Posess(Entity newEntity)
        {
            controlledEntity = newEntity;
        }

        public void Unposess()
        {
            controlledEntity = null;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (controlledEntity != null)
            {
                if (cameraAttachToEntity)
                {
                    Camera.cameraPosition = new Vector2(controlledEntity.rect.Location.X - Main.GetGame().Window.ClientBounds.Width / 2, controlledEntity.rect.Location.Y - Main.GetGame().Window.ClientBounds.Height / 2);
                }

                int inputLR = 0;

                if(Keyboard.GetState().IsKeyDown(Keys.A))
                {
                    inputLR = -1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.D))
                {
                    inputLR = 1;
                }
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    ((Character)controlledEntity).Jump();
                }

                controlledEntity.AxisInput(inputLR);
            }
        }
    }
}
