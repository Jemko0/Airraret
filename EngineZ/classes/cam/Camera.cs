using Microsoft.Xna.Framework;

namespace EngineZ.classes.cam
{
    public class Camera
    {
        public static Vector2 cameraPosition;
        public static Rectangle viewBounds;

        public Camera()
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

        public static bool isInView(Rectangle rect)
        {
            return viewBounds.Contains(rect);
        }
    }
}
