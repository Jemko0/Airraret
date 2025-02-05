


using EngineZ.Events;
using EngineZ.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace EngineZ.UI
{
    public class WButton : Widget
    {
        public WButton(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
            
        }

        public override void Construct()
        {
            InputHandler.mouseClickHandler += OnGlobalClick;
        }

        private void OnGlobalClick(Events.Mouse.MouseClickEventArgs args)
        {
            if (isHovered())
            {
                OnWidgetClicked(args);
            }
        }

        private void OnWidgetClicked(Mouse.MouseClickEventArgs args)
        {
            throw new Exception("fkaoisfk");
        }

        public override unsafe void Draw(ref SpriteBatch spriteBatch)
        {
            var tex = Main.GetGame().Content.Load<Texture2D>("Textures/UI/btn_basic");
            spriteBatch.Draw(tex, geometry, Color.White);
            spriteBatch.DrawString(Airraret.gameFont, "ButtonText", new Vector2(geometry.Center.X, geometry.Center.Y), Color.White);
        }
    }
}
