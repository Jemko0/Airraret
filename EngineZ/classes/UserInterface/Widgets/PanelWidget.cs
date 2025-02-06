using EngineZ.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace EngineZ.UI
{
    public class PanelWidget : Widget
    {
        private protected List<Widget> children = new List<Widget>();
        private protected bool relativizeInheritWH = false;
        public PanelWidget(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }

        public PanelWidget(HUD ownerHUD, Rectangle renderTransform, EWidgetAlignment widgetAlignment) : base(ownerHUD, renderTransform, widgetAlignment)
        {
        }

        public List<Widget> GetChildren()
        {
            return children;
        }

        public void AddChild(Widget w)
        {
            w.RelativizeGeometry(this, relativizeInheritWH);
            children.Add(w);
        }


        public void RemoveChild(Widget w)
        {
            w.DestroyWidget();
            children.Remove(w);
        }

        public Widget RemoveFromParent(Widget w)
        {
            children.Remove(w);
            return w;
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            base.Draw(ref spriteBatch);
            DrawChildren(ref spriteBatch);
        }

        public virtual void DrawChildren(ref SpriteBatch spriteBatch)
        {
            foreach (Widget w in children)
            {
                w.RelativizeGeometry(this, relativizeInheritWH);
                w.UpdateScale();
                w.Draw(ref spriteBatch);
            }
        }
    }
}
