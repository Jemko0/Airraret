

using EngineZ.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EngineZ.UI
{
    public class UWTitleScreen : PanelWidget
    {
        public UWTitleScreen(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }

        public UWTitleScreen(HUD ownerHUD, Rectangle renderTransform, EWidgetAlignment widgetAlignment) : base(ownerHUD, renderTransform, widgetAlignment)
        {
        }
        WVerticalBox verticalBox;
        public override void Construct()
        {
            verticalBox = HUD.CreateWidget<WVerticalBox>(ownerHUD, new Rectangle(200, 0, 400, 800));
            var btn = HUD.CreateWidget<WButton>(ownerHUD, new Rectangle(0, 0, 150, 72));
            var btnText = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 0), "Play", ETextJustification.Center);
            btn.AddChild(btnText);
            verticalBox.AddChild(btn);

            btn = HUD.CreateWidget<WButton>(ownerHUD, new Rectangle(0, 0, 150, 72));
            btnText = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 0), "Settings", ETextJustification.Center);
            btn.AddChild(btnText);
            verticalBox.AddChild(btn);
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            verticalBox.origin = new Vector2(WidgetAlignments.GetFullScreenWidget().Width / 2, 400);
            base.Draw(ref spriteBatch);

            /*
            Texture2D rect = new Texture2D(Main.GetGame().GraphicsDevice, 1, 1);
            rect.SetData(new Color[] { Color.Yellow });
            spriteBatch.Draw(rect, scaledGeometry, Color.White);
            */
        }
    }
}
