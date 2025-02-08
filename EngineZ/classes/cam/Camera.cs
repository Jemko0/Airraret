using EngineZ.classes.world;
using EngineZ.UI;
using Microsoft.Xna.Framework;
using System;

namespace EngineZ.classes.cam
{
    public class Camera
    {
        public static Vector2 cameraPosition;
        public static Rectangle viewBounds;

        public Camera()
        {
            UpdateViewBounds();
            Main.GetGame().Window.ClientSizeChanged += Window_ClientSizeChanged;
        }

        private void Window_ClientSizeChanged(object sender, System.EventArgs e)
        {
            UpdateViewBounds();
        }

        public void UpdateViewBounds()
        {
            viewBounds.X = 0;
            viewBounds.Y = 0;
            viewBounds.Width = Main.GetGame().Window.ClientBounds.Width;
            viewBounds.Height = Main.GetGame().Window.ClientBounds.Height;
        }

        public static Vector2 GetTransformed(Vector2 rawPos)
        {
            rawPos.X -= cameraPosition.X;
            rawPos.Y -= cameraPosition.Y;
            return rawPos;
        }

        public static float ScaleFloatToDPI(float value)
        {
            return value * Main.GetGame().GraphicsDevice.Viewport.Height / 1080f;
        }

        public static Rectangle ScaleRectToDPI(Rectangle rect)
        {
            float scale = Main.GetGame().GraphicsDevice.Viewport.Height / 1080f;
            
            // Get the center of the screen
            float screenCenterX = Main.GetGame().Window.ClientBounds.Width / 2f;
            float screenCenterY = Main.GetGame().Window.ClientBounds.Height / 2f;

            // Calculate position relative to screen center
            float relativeX = rect.X - screenCenterX;
            float relativeY = rect.Y - screenCenterY;

            // Scale the relative position
            float scaledRelativeX = relativeX * scale;
            float scaledRelativeY = relativeY * scale;

            // Convert back to screen coordinates
            Rectangle scaledRect = new Rectangle();
            scaledRect.X = (int)(screenCenterX + scaledRelativeX);
            scaledRect.Y = (int)(screenCenterY + scaledRelativeY);
            scaledRect.Width = (int)(rect.Width * scale);
            scaledRect.Height = (int)(rect.Height * scale);

            return scaledRect;
        }

        public static bool isInView(Rectangle rect)
        {
            return viewBounds.Contains(rect);
        }

        public static Vector2 ScreenToWorld(float screenX, float screenY)
        {
            // Get screen center coordinates
            float screenCenterX = Main.GetGame().Window.ClientBounds.Width / 2f;
            float screenCenterY = Main.GetGame().Window.ClientBounds.Height / 2f;

            // Calculate position relative to screen center
            float relativeX = screenX - screenCenterX;
            float relativeY = screenY - screenCenterY;

            // Scale the coordinates based on DPI (divide by scale since we're converting from screen to world)
            float scale = HUD.DPIScale;
            relativeX /= scale;
            relativeY /= scale;

            float worldX = relativeX + Camera.cameraPosition.X + screenCenterX;
            float worldY = relativeY + Camera.cameraPosition.Y + screenCenterY;

            return new Vector2(worldX, worldY);
        }

        public static Vector2 WorldToTile(float worldX, float worldY)
        {
            int tileX = (int)Math.Floor(worldX / World.TILESIZE) * World.TILESIZE;
            int tileY = (int)Math.Floor(worldY / World.TILESIZE) * World.TILESIZE;
            return new Vector2(tileX, tileY);
        }

        public static Vector2 ScreenToTile(float screenX, float screenY)
        {
            Vector2 worldPos = ScreenToWorld(screenX, screenY);
            return WorldToTile(worldPos.X, worldPos.Y);
        }
    }
}
