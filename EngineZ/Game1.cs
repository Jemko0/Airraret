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
using System.Threading;


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
        public Texture2D blackTx;
        public FPSCounter fpsCounter;
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

            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            blackTx = new Texture2D(_graphics.GraphicsDevice, 1, 1);
            blackTx.SetData(new Color[] { Color.Black });

            clientCamera = new Camera();

            fpsCounter = new FPSCounter(this);
            Components.Add(fpsCounter);

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

            //Thread.Sleep(10);
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                Camera.cameraPosition.Y -= 64;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                Camera.cameraPosition.Y += 64;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                Camera.cameraPosition.X += 64;
            }

            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                Camera.cameraPosition.X -= 64;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            _spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap);

            if(renderWorld)
            {
                int startX = (int)MathUtil.FloatToTileSnap(Camera.cameraPosition.X) + 32;
                int startY = (int)MathUtil.FloatToTileSnap(Camera.cameraPosition.Y) + 32;
                int endX = startX + World.TILESIZE * (int)(116);
                int endY = startY + World.TILESIZE * (int)(64);

                for (int x = startX; x < endX; x += World.TILESIZE)
                {
                    for (int y = startY; y < endY; y += World.TILESIZE)
                    {
                        Vector2 tilePos = new Vector2(x, y);
                        int centerX = Window.ClientBounds.Width / 2;
                        int centerY = Window.ClientBounds.Height / 2;

                        int screenX = (int)(x - Camera.cameraPosition.X) - centerX;
                        int screenY = (int)(y - Camera.cameraPosition.Y) - centerY;
                        int screenSize = World.TILESIZE;
                        

                        screenX = (int)(screenX * HUD.DPIScale);
                        screenY = (int)(screenY * HUD.DPIScale);
                        screenSize = (int)(screenSize * HUD.DPIScale);

                        screenX += centerX;
                        screenY += centerY;

                        Rectangle drawRect = new Rectangle(
                            (int)screenX,
                            (int)screenY,
                            (int)screenSize,
                            (int)screenSize
                        );

                        RenderWall(tilePos, drawRect);
                        RenderTile(tilePos, drawRect);
                        
                        
                    }
                }
            }

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

            _spriteBatch.End();

            clientHUD.DrawWidgets();

            base.Draw(gameTime);
        }

        private void RenderTile(Vector2 tilePos, Rectangle drawRect)
        {

            if(!World.tiles.ContainsKey(tilePos))
            {
                return;
            }

            ETileTypes tileType = World.tiles[tilePos];
            if (tileType == ETileTypes.Air)
            {
                return;
            }
            Lighting.UpdateLighting(tilePos);
            int lightLevel = Lighting.GetLightLevel(tilePos);


            float lightIntensity = lightLevel / 16f;
            Tile tileData = TileID.GetTile(tileType);
            Color lightColor = tileData.tint * lightIntensity;
            lightColor.A = 0xff;

            if (!World.tileFrames.ContainsKey(tilePos))
            {
                Rectangle frame = World.GetTileFrame((int)tilePos.X, (int)tilePos.Y, tileData);
                World.tileFrames[tilePos] = frame;
            }

            _spriteBatch.Draw(tileData.sprite, drawRect, World.tileFrames[tilePos], lightColor);
        }

        void RenderWall(Vector2 tilePos, Rectangle drawRect)
        {
            /*
            if(World.tiles.ContainsKey(tilePos))
            {
                if (TileID.GetTile(World.tiles[tilePos]).hideWall)
                {
                    return;
                }
            }*/

            if(!World.walls.ContainsKey(tilePos))
            {
                return;
            }

            

            EWallTypes wallType = World.walls[tilePos];

            if (wallType == EWallTypes.Air)
            {
                return;
            }

            Lighting.UpdateLighting(tilePos);
            int lightLevel = Lighting.GetLightLevel(tilePos);

            float lightIntensity = lightLevel / 16f;
            Wall wallData = WallID.GetWall(wallType);
            Color lightColor = wallData.tint * lightIntensity;
            lightColor.A = 0xff;

            if (!World.wallFrames.ContainsKey(tilePos))
            {
                Rectangle frame = World.GetWallFrame((int)tilePos.X, (int)tilePos.Y, wallData);
                World.wallFrames[tilePos] = frame;
            }

            _spriteBatch.Draw(wallData.sprite, drawRect, World.wallFrames[tilePos], lightColor);
        }
    }
}
