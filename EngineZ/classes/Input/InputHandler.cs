using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EngineZ.Events.Mouse;

namespace EngineZ.Input
{
    public class InputHandler
    {
        public static event OnMouseClick mouseClickHandler;

        public static void HandleInput()
        {
            ButtonState lastLMBState = ButtonState.Released;

            if(Mouse.GetState().LeftButton == ButtonState.Pressed && lastLMBState != Mouse.GetState().LeftButton)
            {
                lastLMBState = ButtonState.Pressed;
                mouseClickHandler.Invoke(new MouseClickEventArgs(Mouse.GetState().X, Mouse.GetState().Y));
            }
        }
    }
}
