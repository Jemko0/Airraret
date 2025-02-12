using EngineZ.classes.world;
using EngineZ.DataStructures;
using EngineZ.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.UI
{
    public class UWFPSDisplay : PanelWidget
    {
        public UWFPSDisplay(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }
        int verticalBoxWidth = 500;
        WTextBlock delta;
        WTextBlock fps;
        WTextBlock targetElapsed;
        WTextBlock maxElapsed;
        WButton fullbrightButton;
        WTextBlock fullbrightButtonText;
        public UWFPSDisplay(HUD ownerHUD, Rectangle renderTransform, EWidgetAlignment widgetAlignment) : base(ownerHUD, renderTransform, widgetAlignment)
        {
        }

        WVerticalBox vert;
        public override void Construct()
        {
            relativizeInheritWH = false;
            delta = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 64), "delta", ETextJustification.Center);
            fps = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 64), "fps", ETextJustification.Center);
            targetElapsed = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 64), "target", ETextJustification.Center);
            maxElapsed = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 64), "max", ETextJustification.Center);
            fullbrightButton = HUD.CreateWidget<WButton>(ownerHUD, new Rectangle(0, 0, 0, 64));
            fullbrightButtonText = HUD.CreateWidget<WTextBlock>(ownerHUD, new Rectangle(0, 0, 0, 64), "Toggle Fullbright", ETextJustification.Center);
            fullbrightButton.buttonPressed += ToggleFullbright;
            vert = HUD.CreateWidget<WVerticalBox>(ownerHUD, new Rectangle(0, 0, verticalBoxWidth, 200));
            vert.AddChild(delta);
            vert.AddChild(fps);
            vert.AddChild(targetElapsed);
            vert.AddChild(maxElapsed);

            vert.AddChild(fullbrightButton);
            fullbrightButton.AddChild(fullbrightButtonText);


            AddChild(vert);

            base.Construct();
        }

        private void ToggleFullbright(ButtonInteractionEventArgs args)
        {
            Lighting.FULLBRIGHT = !Lighting.FULLBRIGHT;
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            delta.text = "Delta ms: " + Main.GetGame().InactiveSleepTime.TotalMilliseconds.ToString();
            fps.text = "FPS: " + Main.GetGame().fpsCounter.fps;
            targetElapsed.text = "Target Elapsed ms: " + Main.GetGame().TargetElapsedTime.TotalMilliseconds.ToString();
            maxElapsed.text = "Max Elapsed ms: " + Main.GetGame().MaxElapsedTime.TotalMilliseconds.ToString();
            origin = new Vector2(0, 200);
            base.Draw(ref spriteBatch);
        }
    }
}
