using EngineZ.classes.cam;
using EngineZ.classes.interfaces;
using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.ID;
using EngineZ.Input;
using EngineZ.UI;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EngineZ.Entities
{
    public class Character : Entity
    {
        public float acceleration = 24.0f;
        public float maxWalkSpeed = 350.0f;
        public float jumpPower = 800.0f;
        public float groundFriction = 6.0f;
        public float airControl = 0.5f;
        public float airDecel = 4f;

        private const float skinWidth = 0.1f;
        public float lastInputLR = 0.0f;
        public Character(Game game, EEntityTypes initType) : base(game, initType)
        {
            InputHandler.onLeftMousePressed += MouseClickedOnWorld;
        }

        private void MouseClickedOnWorld(Events.MouseEvent.MouseClickEventArgs args)
        {
            Vector2 tilePos = Camera.ScreenToTile(args.X, args.Y);
            World.SetTile((int)tilePos.X, (int)tilePos.Y, ETileTypes.Air);
        }

        private void MoveWithCollision(ref int moveX, ref int moveY)
        {
            // Handle X movement first
            if (moveX != 0)
            {
                int direction = Math.Sign(moveX);
                float rayLength = Math.Abs(moveX);

                // Check three points along the side we're moving towards
                Vector2[] rayStarts = new Vector2[]
                {
                    new Vector2(direction == 1 ? rect.Right : rect.Left, rect.Top + skinWidth),
                    new Vector2(direction == 1 ? rect.Right : rect.Left, rect.Top + rect.Height * 0.5f),
                    new Vector2(direction == 1 ? rect.Right : rect.Left, rect.Bottom - skinWidth)
                };

                float shortestHit = rayLength;
                bool collision = false;

                foreach (var rayStart in rayStarts)
                {
                    if (CastRay(rayStart, new Vector2(direction, 0), rayLength, out float hitDistance))
                    {
                        shortestHit = Math.Min(shortestHit, hitDistance);
                        collision = true;
                    }
                }

                if (collision)
                {
                    moveX = (int)(direction * (Math.Max(0, shortestHit - skinWidth)));
                    velocity.X = 0;
                }
            }

            rect.X += moveX;

            // Handle Y movement
            if (moveY != 0)
            {
                int direction = Math.Sign(moveY);
                float rayLength = Math.Abs(moveY);

                // Check three points along the side we're moving towards
                Vector2[] rayStarts = new Vector2[]
                {
                    new Vector2(rect.Left + skinWidth, direction == 1 ? rect.Bottom : rect.Top),
                    new Vector2(rect.Left + rect.Width * 0.5f, direction == 1 ? rect.Bottom : rect.Top),
                    new Vector2(rect.Right - skinWidth, direction == 1 ? rect.Bottom : rect.Top)
                };

                float shortestHit = rayLength;
                bool collision = false;

                foreach (var rayStart in rayStarts)
                {
                    if (CastRay(rayStart, new Vector2(0, direction), rayLength, out float hitDistance))
                    {
                        shortestHit = Math.Min(shortestHit, hitDistance);
                        collision = true;
                    }
                }

                if (collision)
                {
                    moveY = (int)(direction * (Math.Max(0, shortestHit - skinWidth)));
                    velocity.Y = 0;
                }
            }

            rect.Y += moveY;
        }

        private bool CastRay(Vector2 start, Vector2 direction, float length, out float hitDistance)
        {
            hitDistance = length;
            Vector2 end = start + direction * length;

            // Convert to tile coordinates
            Vector2 startTile = new Vector2(
                (float)Math.Floor(start.X / World.TILESIZE),
                (float)Math.Floor(start.Y / World.TILESIZE));
            Vector2 endTile = new Vector2(
                (float)Math.Floor(end.X / World.TILESIZE),
                (float)Math.Floor(end.Y / World.TILESIZE));

            // Check each tile along the ray
            int minX = (int)Math.Min(startTile.X, endTile.X);
            int maxX = (int)Math.Max(startTile.X, endTile.X);
            int minY = (int)Math.Min(startTile.Y, endTile.Y);
            int maxY = (int)Math.Max(startTile.Y, endTile.Y);

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Vector2 tilePos = new Vector2(x * World.TILESIZE, y * World.TILESIZE);

                    if (World.tiles.TryGetValue(tilePos, out ETileTypes tileType))
                    {
                        Tile tileData = TileID.GetTile(tileType);
                        if (tileData.valid && tileData.collide)
                        {
                            Rectangle tileRect = new Rectangle(
                                (int)tilePos.X,
                                (int)tilePos.Y,
                                World.TILESIZE,
                                World.TILESIZE);

                            if (RayIntersectsRect(start, direction, tileRect, out float distance))
                            {
                                if (distance < hitDistance)
                                {
                                    hitDistance = distance;
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }

        private bool RayIntersectsRect(Vector2 origin, Vector2 direction, Rectangle rect, out float distance)
        {
            distance = 0f;
            Vector2 min = new Vector2(rect.Left, rect.Top);
            Vector2 max = new Vector2(rect.Right, rect.Bottom);

            float tmin = float.NegativeInfinity;
            float tmax = float.PositiveInfinity;

            // Check X axis
            if (Math.Abs(direction.X) < float.Epsilon)
            {
                if (origin.X < min.X || origin.X > max.X)
                    return false;
            }
            else
            {
                float invD = 1f / direction.X;
                float t1 = (min.X - origin.X) * invD;
                float t2 = (max.X - origin.X) * invD;

                if (t1 > t2)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);

                if (tmin > tmax)
                    return false;
            }

            // Check Y axis
            if (Math.Abs(direction.Y) < float.Epsilon)
            {
                if (origin.Y < min.Y || origin.Y > max.Y)
                    return false;
            }
            else
            {
                float invD = 1f / direction.Y;
                float t1 = (min.Y - origin.Y) * invD;
                float t2 = (max.Y - origin.Y) * invD;

                if (t1 > t2)
                {
                    float temp = t1;
                    t1 = t2;
                    t2 = temp;
                }

                tmin = Math.Max(tmin, t1);
                tmax = Math.Min(tmax, t2);

                if (tmin > tmax)
                    return false;
            }

            distance = tmin;
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Move();
        }

        private bool IsOnGround()
        {
            // Cast three rays down from the bottom of the character
            Vector2[] rayStarts = new Vector2[]
            {
        new Vector2(rect.Left + skinWidth, rect.Bottom),
        new Vector2(rect.Left + rect.Width * 0.5f, rect.Bottom),
        new Vector2(rect.Right - skinWidth, rect.Bottom)
            };

            float groundCheckDistance = 2f; // Small distance to check below feet

            foreach (var rayStart in rayStarts)
            {
                if (CastRay(rayStart, Vector2.UnitY, groundCheckDistance, out float hitDistance))
                {
                    return true;
                }
            }

            return false;
        }

        public void Jump()
        {
            if (IsOnGround())
            {
                velocity.Y = -jumpPower * Main.delta;
            }
        }

        public void Movement(float inputLR)
        {
            if (!IsOnGround())
            {
                inputLR *= airControl;
                velocity.X /= 1 + airDecel * Main.delta;
            }

            lastInputLR = inputLR;
            velocity.X = Math.Clamp(velocity.X + inputLR * acceleration * Main.delta, -maxWalkSpeed * Main.delta, maxWalkSpeed * Main.delta);
        }

        public void Move()
        {
            velocity.Y += 9.81f * (float)Main.GetGame().TargetElapsedTime.TotalSeconds; // Gravity

            // Convert velocity to movement amounts
            int moveX = (int)velocity.X;
            int moveY = (int)velocity.Y;
            MoveWithCollision(ref moveX, ref moveY);

            if (Math.Abs(velocity.X) > 0 && IsOnGround() && lastInputLR == 0)
            {
                velocity.X /= 1 + groundFriction * Main.delta;
                if (Math.Abs(velocity.X) < 0.01f)
                    velocity.X = 0;
            }

        }

        public override void AxisInput(float axisVal)
        {
            Movement(axisVal);
        }
    }
}