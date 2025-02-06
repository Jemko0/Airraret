using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EngineZ.UI
{
    public class WVerticalBox : PanelWidget
    {
        public bool dbg = true;
        public bool vbChildrenInheritWidth = true;

        public WVerticalBox(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
            relativizeInheritWH = false;
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            Texture2D rect = new Texture2D(Main.GetGame().GraphicsDevice, 1, 1);
            rect.SetData(new Color[] { Color.Red });

            if(dbg)
            {
                spriteBatch.Draw(rect, scaledGeometry, Color.White);
            }

            base.Draw(ref spriteBatch);
        }

        public override void DrawChildren(ref SpriteBatch spriteBatch)
        {
            int childidx = children.Count; //reverse order bc it makes it the correct order

            foreach (Widget w in children)
            {
                Rectangle childGeo = w.GetGeometry();
                
                childGeo.X = GetGeometry().X;
                childGeo.Y = GetGeometry().Y + (int)(w.GetGeometry().Height * childidx * HUD.DPIScale);

                if(vbChildrenInheritWidth)
                {
                    childGeo.Width = GetGeometry().Width;
                }

                Vector2 childOrigin = new Vector2(origin.X, origin.Y);

                w.origin = childOrigin;
                w.SetGeometry(childGeo);
                w.Draw(ref spriteBatch);
                childidx--;
            }
        }
    }
}