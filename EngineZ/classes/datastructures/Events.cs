using EngineZ.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;

namespace EngineZ.Events
{
    public partial class MouseEvent
    {
        public delegate void OnMouseClick(MouseClickEventArgs args);
        public delegate void OnMouseRelease(MouseClickEventArgs args);
        public class MouseClickEventArgs : EventArgs
        {
            public MouseClickEventArgs(int x, int y, ButtonState state)
            {
                X = x;
                Y = y;
                this.state = state;
                MousePosition = new Vector2(x, y);
            }

            public ButtonState state;
            public int X;
            public int Y;
            public Vector2 MousePosition;
        }
    }

    public class WidgetDestroyEventArgs
    {
        public Widget destroyedWidget;
        public WidgetDestroyEventArgs(Widget w)
        {
            destroyedWidget = w;
        }
    }
    
}
