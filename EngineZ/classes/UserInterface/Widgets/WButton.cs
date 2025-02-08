
using EngineZ.Events;
using EngineZ.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace EngineZ.UI
{
    public class WButton : PanelWidget
    {
        public WButton(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
            relativizeInheritWH = true;
        }

        public delegate void widgetButtonPressed(ButtonInteractionEventArgs args);
        public delegate void widgetButtonReleased(ButtonInteractionEventArgs args);
        public event widgetButtonPressed buttonPressed;
        public event widgetButtonReleased buttonReleased;

        protected Texture2D buttonTex = Main.GetGame().Content.Load<Texture2D>("Textures/UI/btn_basic");
        protected Texture2D buttonHoveredTex = new Texture2D(Main.GetGame().GraphicsDevice, 1, 1);
        public override void Construct()
        {
            buttonHoveredTex.SetData(new Color[]{Color.SlateGray});
            InputHandler.onLeftMousePressed += OnGlobalClick;
            InputHandler.onLeftMouseReleased += OnGlobalClick;
        }

        private void OnGlobalClick(Events.MouseEvent.MouseClickEventArgs args)
        {
            if (isHovered)
            {
                OnWidgetClicked(args);
            }
        }

        private void OnWidgetClicked(Events.MouseEvent.MouseClickEventArgs args)
        {

            if(args.state == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                if(buttonPressed != null)
                {
                    buttonPressed.Invoke(new ButtonInteractionEventArgs(args.state));
                }
                
            }
            else
            {
                if (buttonReleased != null)
                {
                    buttonReleased.Invoke(new ButtonInteractionEventArgs(args.state));
                }
            }
        }

        public override unsafe void Draw(ref SpriteBatch spriteBatch)
        {
            Texture2D drawTexture = isHovered ? buttonHoveredTex : buttonTex;
            spriteBatch.Draw(drawTexture, scaledGeometry, Color.White);
            base.Draw(ref spriteBatch);
        }

        public override void ClearDelegates()
        {
            buttonPressed = null;
            buttonReleased = null;
        }
    }

    public class ButtonInteractionEventArgs
    {
        public ButtonState state;
        public ButtonInteractionEventArgs(ButtonState state)
        {
            this.state = state;
        }
    }
}