using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EngineZ.UI
{
    public static class Primitives
    {
    }

    public static class WidgetAlignments
    {
        public static Vector2 GetScreenCenter(Rectangle parentGeo)
        {
            Rectangle center = Main.GetGame().Window.ClientBounds;
            center.X = 0;
            center.Y = 0;
            return new Vector2(parentGeo.X + center.Width / 2, parentGeo.Y + center.Height / 2);
        }

        public static Rectangle GetFullScreenWidget()
        {
            Rectangle r = Main.GetGame().Window.ClientBounds;
            r.X = 0;
            r.Y = 0;
            return r;
        }
    }
}
