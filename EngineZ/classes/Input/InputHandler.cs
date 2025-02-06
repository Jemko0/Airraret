using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using static EngineZ.Events.Mouse;

namespace EngineZ.Input
{
    public class InputHandler
    {
        public static event OnMouseClick onLeftMousePressed;
        public static event OnMouseRelease onLeftMouseReleased;

        public static ButtonState lastLMBState = ButtonState.Released;
        public static void HandleInput()
        {
            MouseClickEventArgs newArgs = new MouseClickEventArgs(Mouse.GetState().X, Mouse.GetState().Y, ButtonState.Pressed);


            //PRESS
            if (lastLMBState != ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                lastLMBState = ButtonState.Pressed;
                newArgs.state = lastLMBState;
                onLeftMousePressed.Invoke(newArgs);
            }

            //RELEASE
            if(lastLMBState != ButtonState.Released && Mouse.GetState().LeftButton == ButtonState.Released)
            {
                lastLMBState = ButtonState.Released;
                newArgs.state = lastLMBState;
                onLeftMouseReleased.Invoke(newArgs);
            }
        }
    }
}
