using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.Events
{
    public partial class Mouse
    {
        public delegate void OnMouseClick(MouseClickEventArgs args);

        public class MouseClickEventArgs : EventArgs
        {
            public MouseClickEventArgs(int x, int y)
            {
                X = x;
                Y = y;
                MousePosition = new Vector2(x, y);
            }

            public int X;
            public int Y;
            public Vector2 MousePosition;
        }
    }


    
}
