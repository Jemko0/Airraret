using EngineZ.classes.cam;
using EngineZ.classes.gameplay;
using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.Entities;
using EngineZ.ID;
using EngineZ.Input;
using EngineZ.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineZ
{
    public class Airraret : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Entity[] entities = new Entity[255];
        private World world;
        private Camera clientCamera;
        private HUD clientHUD;
        private PlayerController LocalPlayerController;
        public static SpriteFont gameFont;
        public Airraret()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = false;
            Window.Title = "lol";

            LocalPlayerController = new PlayerController(this, 0);

            world = new World();
            world.FillWorld(50);

            clientCamera = new Camera();

            clientHUD = new HUD();
            base.Initialize();

        }
        #region createDestroy
        public static Entity? CreateEntity<T>(EntityTypes type) where T : Entity
        {
            T e = (T)Activator.CreateInstance(typeof(T), Main.GetGame(), type);
            if (RegisterEntity(e))
            {
                Main.GetGame().Components.Add(e);
                return e;
            }
            
            return null;
        }

        public static void DestroyEntity(ref Entity e)
        {
            Main.GetGame().Components.Remove(e);
            e.Dispose();

            return;
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
        #endregion
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            clientHUD.InitHUD(ref _spriteBatch, ref _graphics);

            gameFont = Content.Load<SpriteFont>("Fonts/Andy");

            Entity b = CreateEntity<Character>(EntityTypes.Player);
            b.SetLocation(50, -500);

            LocalPlayerController.Posess(b);
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler.HandleInput();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.cameraPosition.Y -= 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.cameraPosition.Y += 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.cameraPosition.X += 5;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.cameraPosition.X -= 5;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Texture);

            //Entity Render
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == null)
                    continue;

                Rectangle renderRect = new Rectangle();
                renderRect = entities[i].GetRect();
                Vector2 newPos = Camera.GetTransformed(new Vector2(renderRect.X, renderRect.Y));
                renderRect.X = (int)newPos.X;
                renderRect.Y = (int)newPos.Y;

                renderRect = Camera.ScaleRectToDPI(renderRect);
                _spriteBatch.Draw(entities[i].type.sprite, renderRect, entities[i].type.tint);
            }

            //World Render
            float scale = Main.GetGame().GraphicsDevice.Viewport.Height / 1080f;
            float scaledTileSize = (float)Math.Ceiling(World.TILESIZE * scale);


            // For Y, we need to consider that positive Y is downward
            int startX = (int)(Camera.cameraPosition.X / World.TILESIZE);
            int startY = (int)(Camera.cameraPosition.Y / World.TILESIZE);
            int endX = startX + Main.GetGame().Window.ClientBounds.Width / World.TILESIZE;
            int endY = startY + Main.GetGame().Window.ClientBounds.Height / World.TILESIZE;

            // Get screen center
            float screenCenterX = Main.GetGame().Window.ClientBounds.Width / 2f;
            float screenCenterY = Main.GetGame().Window.ClientBounds.Height / 2f;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Vector2 tilePos = new Vector2(x * World.TILESIZE, y * World.TILESIZE);
                    
                    if (World.tiles.ContainsKey(tilePos))
                    {
                        // Calculate position relative to screen center, Y increases downward
                        float relativeX = x * World.TILESIZE - Camera.cameraPosition.X - screenCenterX;
                        float relativeY = y * World.TILESIZE - Camera.cameraPosition.Y - screenCenterY;

                        // Scale the relative position
                        float scaledX = relativeX * scale;
                        float scaledY = relativeY * scale;

                        Rectangle drawRect = new Rectangle(
                            (int)(screenCenterX + scaledX),
                            (int)(screenCenterY + scaledY),
                            (int)scaledTileSize,
                            (int)scaledTileSize
                        );

                        TileTypes tileType = World.tiles[tilePos];
                        Tile tileData = TileID.GetTile(tileType);
                        _spriteBatch.Draw(tileData.sprite, drawRect, Color.White);
                    }
                }
            }

            _spriteBatch.DrawString(gameFont, "test", new Vector2(25, 25), Color.Black);
            _spriteBatch.End();
            
            clientHUD.DrawWidgets();

            base.Draw(gameTime);

        }
    }
}
