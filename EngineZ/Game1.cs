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
using static System.Drawing.RectangleF;


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

                const int TILE_PAD = 0; //Render TILE_PAD amount more tiles outside of screen (for lighting)

                const int TILES_X = 118 + TILE_PAD;
                const int TILES_Y = 66  + TILE_PAD;

                int centerX = Window.ClientBounds.Width / 2;
                int centerY = Window.ClientBounds.Height / 2;
                
                int startX = (int)MathUtil.FloatToTileSnap(Camera.cameraPosition.X - (TILES_X * World.TILESIZE / 2));
                int startY = (int)MathUtil.FloatToTileSnap(Camera.cameraPosition.Y - (TILES_Y * World.TILESIZE / 2));
                int endX = startX + World.TILESIZE * TILES_X;
                int endY = startY + World.TILESIZE * TILES_Y;

                for (int x = startX; x < endX; x += World.TILESIZE)
                {
                    for (int y = startY; y < endY; y += World.TILESIZE)
                    {
                        Vector2 tilePos = new Vector2(x, y);
                        
                        int screenX = (int)(x - Camera.cameraPosition.X);
                        int screenY = (int)(y - Camera.cameraPosition.Y);
                        
                        screenX = (int)(screenX * HUD.DPIScale);
                        screenY = (int)(screenY * HUD.DPIScale);
                        int screenSize = (int)(World.TILESIZE * HUD.DPIScale) + 1; //1px extra to prevent gaps
                        
                        screenX += centerX;
                        screenY += centerY;

                        Rectangle drawRect = new Rectangle(
                            screenX,
                            screenY,
                            screenSize,
                            screenSize
                        );

                        Lighting.UpdateLighting(tilePos);

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

                System.Drawing.RectangleF rectF = entities[i].GetRect();
                Rectangle entityRect = new Rectangle((int)rectF.X, (int)rectF.Y, (int)rectF.Width, (int)rectF.Height);
                
                int screenX = (int)(entityRect.X - Camera.cameraPosition.X);
                int screenY = (int)(entityRect.Y - Camera.cameraPosition.Y);
                
                screenX = (int)(screenX * HUD.DPIScale);
                screenY = (int)(screenY * HUD.DPIScale);
                int screenWidth = (int)(entityRect.Width * HUD.DPIScale);
                int screenHeight = (int)(entityRect.Height * HUD.DPIScale);
                
                screenX += Window.ClientBounds.Width / 2;
                screenY += Window.ClientBounds.Height / 2;

                Rectangle drawRect = new Rectangle(
                    screenX,
                    screenY,
                    screenWidth,
                    screenHeight
                );

                int lightLevel = Lighting.GetLightLevel(new Vector2(MathUtil.FloatToTileSnap(entityRect.X), MathUtil.FloatToTileSnap(entityRect.Y)));

                Color finalColor = entities[i].type.tint * ((float)lightLevel / Lighting.MAX_LIGHT);
                finalColor.A = 0xff;

                _spriteBatch.Draw(entities[i].type.sprite, drawRect, finalColor);
            }

            _spriteBatch.End();

            clientHUD.DrawWidgets();

            base.Draw(gameTime);
        }

        void RenderTile(Vector2 tilePos, Rectangle drawRect)
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
            int lightLevel = Lighting.GetLightLevel(tilePos);


            float lightIntensity = (float)(lightLevel / (float)Lighting.MAX_LIGHT);
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

            if (Lighting.GetLightLevel(tilePos) == Lighting.MIN_LIGHT && World.IsValidTile(tilePos))
            {
                return;
            }
            

            EWallTypes wallType = World.walls[tilePos];

            if (wallType == EWallTypes.Air)
            {
                return;
            }

            int lightLevel = Lighting.GetLightLevel(tilePos);

            float lightIntensity = (float)(lightLevel / (float)Lighting.MAX_LIGHT);
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
