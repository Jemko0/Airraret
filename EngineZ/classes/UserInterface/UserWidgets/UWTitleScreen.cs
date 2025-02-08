

using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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
        WButton btn;
        WButton btn2;
        public override void Construct()
        {
            verticalBox = HUD.CreateWidget<WVerticalBox>(ownerHUD, new Rectangle(0, 0, 400, 800));
            btn = HUD.CreateWidget<WButton>(ownerHUD, new Rectangle(0, 0, 150, 72));
            var btnText = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 0), "Play", ETextJustification.Center);
            btn.buttonPressed += PlayButtonPressed;
            btn.AddChild(btnText);
            verticalBox.AddChild(btn);

            btn2 = HUD.CreateWidget<WButton>(ownerHUD, new Rectangle(0, 0, 150, 72));
            var btnText2 = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 0), "Settings", ETextJustification.Center);
            btn2.buttonPressed += SettingsButtonPressed;
            btn2.AddChild(btnText2);
            verticalBox.AddChild(btn2);

            AddChild(verticalBox);
            Logger.Log(ELogCategory.LogUI, btn2.GetScaledGeometry());
        }

        private void SettingsButtonPressed(ButtonInteractionEventArgs args)
        {
            
        }

        private void PlayButtonPressed(ButtonInteractionEventArgs args)
        {
            World.CreateWorld();
            HUD.CreateWidget<UWWorldGenProgress>(ownerHUD, new Rectangle(0, 0, 1, 1), EWidgetAlignment.TopLeft);
            DestroyWidget();
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            //GetGeometry().X = (int)(250 * HUD.DPIScale);
            origin = new Vector2((WidgetAlignments.GetFullScreenWidget().Width / 2) - 200 * HUD.DPIScale, 500);
            verticalBox.origin = origin;
            base.Draw(ref spriteBatch);
        }
    }
}
