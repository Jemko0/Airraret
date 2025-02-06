using EngineZ.classes.interfaces;
using EngineZ.DataStructures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace EngineZ.UI
{
    public class HUD : IHUD
    {
        protected unsafe SpriteBatch spriteBatch;
        protected unsafe GraphicsDeviceManager GDM;
        private static List<Widget> activeWidgets = new List<Widget>();
        public HUD()
        {
            WButton b = CreateWidget<WButton>(this, new Rectangle(400, 100, 250, 100));
            WTextBlock tb = CreateWidget<WTextBlock>(this, new Rectangle(0, 0, 100, 100), "Testtext", TextJustification.Right);
            b.AddChild(tb);
        }

        public unsafe void InitHUD(ref SpriteBatch spriteBatch, ref GraphicsDeviceManager gdm)
        {
            this.spriteBatch = spriteBatch;
            GDM = gdm;
        }

        public static T CreateWidget<T>(params object?[]? args) where T : Widget
        {
            T newWidget = (T)Activator.CreateInstance(typeof(T), args);
            newWidget.Construct();
            activeWidgets.Add(newWidget);
            return newWidget;
        }

        public static void DestroyWidget(Widget w)
        {
            activeWidgets.Remove(w);
            w.Dispose();
        }

        public unsafe void DrawWidgets()
        {
            spriteBatch.Begin(SpriteSortMode.Immediate);
            foreach (Widget w in activeWidgets)
            {
                if (w.suppressDraw)
                    continue;

                w.Draw(ref spriteBatch);
            }
            spriteBatch.End();
        }
    }
}
