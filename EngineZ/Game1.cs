using EngineZ.classes.cam;
using EngineZ.classes.gameplay;
using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.Entities;
using EngineZ.ID;
using EngineZ.Input;
using EngineZ.Music;
using EngineZ.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace EngineZ
{
    public class Airraret : Game
    {
        private GraphicsDeviceManager _graphics;
        public SpriteBatch _spriteBatch;
        public static Entity[] entities = new Entity[255];
        public World world;
        private Camera clientCamera;
        private HUD clientHUD;
        public PlayerController LocalPlayerController;
        public static SpriteFont gameFont24;
        public static bool renderWorld;
        public MusicManager musicManager;

        public Airraret()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(80000);
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = true;
            Window.Title = "Airarret";
            
            clientCamera = new Camera();
            base.Initialize();
        }

        public void CreatePlayer(Vector2? newPosition)
        {
            LocalPlayerController = new PlayerController(this, 0);
            var player = CreateEntity<Character>(EEntityTypes.Player);
            LocalPlayerController.Posess(player);

            if(newPosition != null)
            {
                player.SetLocation((Vector2)newPosition);
            }
        }

        #region createDestroy
        public static Entity? CreateEntity<T>(EEntityTypes type) where T : Entity
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
            MusicManager.InitMusic();
            musicManager = new MusicManager();
            musicManager.SetNext("Title");

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            gameFont24 = Content.Load<SpriteFont>("Fonts/Andy");

            clientHUD = new HUD();
            clientHUD.InitHUD(ref _spriteBatch, ref _graphics);

            HUD.CreateWidget<UWTitleScreen>(clientHUD, new Rectangle(0, 0, 1, 1), EWidgetAlignment.TopLeft);
        }

        protected override void Update(GameTime gameTime)
        {
            InputHandler.HandleInput();
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if(Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.cameraPosition.Y -= 25;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.cameraPosition.Y += 25;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.cameraPosition.X += 25;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.cameraPosition.X -= 25;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);


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
            if(renderWorld)
            {
                float scale = HUD.DPIScale;
                float scaledTileSize = (float)Math.Ceiling(World.TILESIZE * scale);

                int startX = (int)(Camera.cameraPosition.X / World.TILESIZE);
                int startY = (int)(Camera.cameraPosition.Y / World.TILESIZE);
                int endX = startX + Main.GetGame().Window.ClientBounds.Width / World.TILESIZE;
                int endY = startY + Main.GetGame().Window.ClientBounds.Height / World.TILESIZE;

                float screenCenterX = Main.GetGame().Window.ClientBounds.Width / 2f;
                float screenCenterY = Main.GetGame().Window.ClientBounds.Height / 2f;

                for (int x = startX; x < endX; x++)
                {
                    for (int y = startY; y < endY; y++)
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

                            ETileTypes tileType = World.tiles[tilePos];
                            if(tileType == ETileTypes.Air)
                            {
                                continue;
                            }
                            Tile tileData = TileID.GetTile(tileType);
                            Rectangle frame = World.GetTileFrame((int)tilePos.X, (int)tilePos.Y, tileData);

                            _spriteBatch.Draw(tileData.sprite, drawRect, frame, tileData.tint);
                        }
                    }
                }
            }

            _spriteBatch.End();

            clientHUD.DrawWidgets();

            base.Draw(gameTime);
        }

        private void LightTile()
        {

        }
    }
}
