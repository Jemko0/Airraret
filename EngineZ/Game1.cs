using EngineZ.classes.cam;
using EngineZ.classes.gameplay;
using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.Entities;
using EngineZ.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineZ
{
    public class Airraret : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        public static Entity[] entities = new Entity[255];
        private World world;
        private Camera clientCamera;
        private PlayerController LocalPlayerController;

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
            LocalPlayerController = new PlayerController(this, 0);
            world = new World();
            world.FillWorld(500);
            clientCamera = new Camera();
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
            Entity b = CreateEntity<Character>(EntityTypes.Player);
            b.SetLocation(50, -500);
            LocalPlayerController.Posess(b);
        }

        protected override void Update(GameTime gameTime)
        {
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
            _spriteBatch.Begin(SpriteSortMode.Deferred);
            for (int i = 0; i < entities.Length; i++)
            {
                if (entities[i] == null)
                    continue;

                Rectangle renderRect = new Rectangle();
                renderRect = entities[i].GetRect();
                Vector2 newPos = Camera.GetTransformed(new Vector2(renderRect.X, renderRect.Y));

                renderRect.X = (int)newPos.X;
                renderRect.Y = (int)newPos.Y;

                _spriteBatch.Draw(entities[i].type.sprite, renderRect, entities[i].type.tint);
            }

            foreach (var (tile, drawTile) in from tile in World.tiles
                                             let drawTile = new Rectangle((int)Camera.GetTransformed(tile.Key).X, (int)Camera.GetTransformed(tile.Key).Y, World.TILESIZE, World.TILESIZE)
                                             where Camera.isInView(drawTile)
                                             select (tile, drawTile))
            {
                _spriteBatch.Draw(TileID.GetTile(tile.Value).sprite, drawTile, Color.White);
            }

            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
