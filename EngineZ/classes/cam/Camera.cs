using Microsoft.Xna.Framework;

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
    }
}
