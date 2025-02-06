using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineZ.UI
{
    public class PanelWidget : Widget
    {
        private List<Widget> children = new List<Widget>();
        public PanelWidget(HUD ownerHUD, Rectangle renderTransform) : base(ownerHUD, renderTransform)
        {
        }

        public List<Widget> GetChildren()
        {
            return children;
        }

        public void AddChild(Widget w)
        {
            w.RelativizeGeometry(this);
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

        public void DrawChildren(ref SpriteBatch spriteBatch)
        {
            foreach (Widget w in children)
            {
                w.Draw(ref spriteBatch);
            }
        }
    }
}
