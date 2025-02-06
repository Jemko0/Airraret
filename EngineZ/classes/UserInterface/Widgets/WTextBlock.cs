using EngineZ.DataStructures;
using EngineZ.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
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
        public TextJustification justification;
        Vector2 renderPosition;
        public WTextBlock(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }

        public WTextBlock(HUD ownerHUD, Rectangle renderTransform, string initialText, TextJustification justification) : base(ownerHUD, renderTransform)
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
                    case TextJustification.Left:
                        return new Vector2(geometry.Left, geometry.Center.Y - Airraret.gameFont28.LineSpacing / 2);

                    case TextJustification.Center:
                        return new Vector2(geometry.Center.X - Airraret.gameFont28.MeasureString(text).Length() / 2, geometry.Center.Y - Airraret.gameFont28.LineSpacing / 2);

                    case TextJustification.Right:
                        return new Vector2(geometry.Right - Airraret.gameFont28.MeasureString(text).Length(), geometry.Center.Y - Airraret.gameFont28.LineSpacing / 2);
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
            spriteBatch.DrawString(Airraret.gameFont28, text, GetJustificationRenderPosition(), Color.White);
        }
    }
}
