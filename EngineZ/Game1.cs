using EngineZ.DataStructures;
using EngineZ.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EngineZ
{
    public class Airraret : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Entity[] entities = new Entity[500];
        public Airraret()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            Window.AllowUserResizing = true;
            Window.Title = "lol";
            base.Initialize();
        }

        public static Entity CreateEntity(EntityTypes type)
        {
            Entity e = new Entity(Main.GetGame(), type);
            RegisterEntity(e);
            Main.GetGame().Components.Add(e);
            
            return e;
        }

        public static bool RegisterEntity(Entity e)
        {
            int foundidx = -1;
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == null)
                {
                    foundidx = i;
                    break;
                }
            }

            if (foundidx != -1)
            {
                entities[foundidx] = e;
            }

            return foundidx != -1;
        }
            
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            CreateEntity(EntityTypes.Default);
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == null)
                    continue;

                _spriteBatch.Draw(entities[i].entity.sprite,
                    new Rectangle((int)entities[i].position.X, (int)entities[i].position.Y, (int)entities[i].entity.dimensions.X, (int)entities[i].entity.dimensions.Y),
                    entities[i].tint);
            }
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
