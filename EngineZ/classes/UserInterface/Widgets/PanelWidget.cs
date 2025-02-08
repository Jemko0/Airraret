using EngineZ.DataStructures;
using EngineZ.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
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
            w.widgetDestroyed += OnChildDestroyed;
            children.Add(w);
        }

        private void OnChildDestroyed(WidgetDestroyEventArgs args)
        {
            RemoveFromParent(args.destroyedWidget);
        }

        public void RemoveFromParent(Widget w)
        {
            w.widgetDestroyed -= OnChildDestroyed;
            w = null;
            children.Remove(w);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            foreach (Widget w in children)
            {
                w.DestroyWidget();
            }
        }

        public override void Draw(ref SpriteBatch spriteBatch)
        {
            base.Draw(ref spriteBatch);
            DrawChildren(ref spriteBatch);
        }

        public virtual void DrawChildren(ref SpriteBatch spriteBatch)
        {
            if (suppressDraw)
                return;

            foreach (Widget w in children)
            {
                if(w.suppressDraw)
                    continue;

                w.RelativizeGeometry(this, relativizeInheritWH);
                w.UpdateScale();
                w.Draw(ref spriteBatch);
            }
        }
    }
}
