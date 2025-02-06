using EngineZ.classes.interfaces;
using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.ID;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.Entities
{
    public class Character : Entity
    {
        public float acceleration = 1;
        public float maxWalkSpeed = 5;
        public float jumpPower = 10;
        public Character(Game game, EEntityTypes initType) : base(game, initType)
        {
        }

        public void WorldCollision()
        {
            Vector2 topLeft = new Vector2(rect.X / World.TILESIZE, rect.Y / World.TILESIZE);
            Vector2 bottomRight = new Vector2((rect.X + rect.Width) / World.TILESIZE, (rect.Y + rect.Height) / World.TILESIZE);

            for (int x = (int)topLeft.X; x <= (int)bottomRight.X; x++)
            {
                for (int y = (int)topLeft.Y; y <= (int)bottomRight.Y; y++)
                {
                    Vector2 tilePos = new Vector2(x * World.TILESIZE, y * World.TILESIZE);
                    
                    if (World.tiles.ContainsKey(tilePos))
                    {
                        ETileTypes tileType = World.tiles[tilePos];
                        Tile tileData = TileID.GetTile(tileType);

                        if (tileData.valid && tileData.collide)
                        {
                            Rectangle tileRect = new Rectangle((int)tilePos.X, (int)tilePos.Y, World.TILESIZE, World.TILESIZE);

                            if (rect.Intersects(tileRect))
                            {
                                float overlapX = Math.Min(rect.Right - tileRect.Left, tileRect.Right - rect.Left);
                                float overlapY = Math.Min(rect.Bottom - tileRect.Top, tileRect.Bottom - rect.Top);

                                if (overlapX < overlapY)
                                {
                                    if (rect.Center.X < tileRect.Center.X)
                                        rect.X = tileRect.Left - rect.Width;
                                    else
                                        rect.X = tileRect.Right;
                                    velocity.X = 0;
                                }
                                else
                                {
                                    if (rect.Center.Y < tileRect.Center.Y)
                                        rect.Y = tileRect.Top - rect.Height;
                                    else
                                        rect.Y = tileRect.Bottom;
                                    velocity.Y = 0;
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            WorldCollision();
            Move();
        }
        
        public void Jump()
        {
            if(velocity.Y == 0)
            {
                velocity.Y = 5;
            }
        }

        public void Movement(float inputLR)
        {
            velocity.X = Math.Clamp(velocity.X + inputLR * acceleration, -maxWalkSpeed, maxWalkSpeed);
        }

        public void Move()
        {
            velocity.Y += 1;

            rect.X += (int)velocity.X;
            rect.Y += (int)velocity.Y;
        }

        public override void AxisInput(float axisVal)
        {
            Movement(axisVal);
        }
    }
}
