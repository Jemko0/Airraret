using EngineZ.DataStructures;
using EngineZ.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace EngineZ.UI
{

    /// <summary>
    /// renders text
    /// </summary>
    public class WTextBlock : Widget
    {
        public string text = "TextBlock";
        public ETextJustification justification;
        Vector2 renderPosition;
        public WTextBlock(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }

        public WTextBlock(HUD ownerHUD, Rectangle renderTransform, string initialText, ETextJustification justification) : base(ownerHUD, renderTransform)
        {
            text = initialText;
            this.justification = justification;
        }

        public override void Construct()
        {
            base.Construct();
        }

        public Vector2 GetJustificationRenderPosition()
        {
            if(renderPosition == Vector2.Zero)
            {
                switch (justification)
                {
                    case ETextJustification.Left:
                        return new Vector2(scaledGeometry.Left, scaledGeometry.Center.Y - Airraret.gameFont24.LineSpacing * HUD.DPIScale / 2);

                    case ETextJustification.Center:
                        return new Vector2(scaledGeometry.Center.X - Airraret.gameFont24.MeasureString(text).Length() * HUD.DPIScale / 2, scaledGeometry.Center.Y - Airraret.gameFont24.LineSpacing * HUD.DPIScale / 2);

                    case ETextJustification.Right:
                        return new Vector2(scaledGeometry.Right - Airraret.gameFont24.MeasureString(text).Length() * HUD.DPIScale, scaledGeometry.Center.Y - Airraret.gameFont24.LineSpacing * HUD.DPIScale / 2);
                }
            }
            else
            {
                return renderPosition;
            }
            return renderPosition;
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            base.Draw(ref spriteBatch);
            spriteBatch.DrawString(Airraret.gameFont24, text, GetJustificationRenderPosition(), Color.White, 0, new Vector2(0, 0), HUD.DPIScale, SpriteEffects.None, 0.0f);
        }
    }
}
