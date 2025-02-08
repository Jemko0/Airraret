using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EngineZ.UI
{
    public class WVerticalBox : PanelWidget
    {
        public bool vbChildrenInheritWidth = true;

        public WVerticalBox(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
            relativizeInheritWH = false;
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            base.Draw(ref spriteBatch);
        }

        public override void DrawChildren(ref SpriteBatch spriteBatch)
        {
            if (suppressDraw)
                return;

            int childidx = children.Count; //reverse order bc it makes it the correct order

            foreach (Widget w in children)
            {
                if (w.suppressDraw)
                    continue;

                Rectangle childGeo = w.GetGeometry();
                
                childGeo.X = GetGeometry().X;
                childGeo.Y = GetGeometry().Y + (int)(w.GetGeometry().Height * childidx * HUD.DPIScale);

                if(vbChildrenInheritWidth)
                {
                    childGeo.Width = GetGeometry().Width;
                }

                Vector2 childOrigin = new Vector2(origin.X, origin.Y + w.GetGeometry().Height);

                w.origin = childOrigin;
                w.SetGeometry(childGeo);
                w.Draw(ref spriteBatch);
                childidx--;
            }
        }
    }
}