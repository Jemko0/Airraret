using EngineZ.classes.interfaces;
using EngineZ.DataStructures;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;

namespace EngineZ.UI
{
    public class HUD : IHUD
    {
        protected unsafe SpriteBatch spriteBatch;
        protected unsafe GraphicsDeviceManager GDM;
        private static List<Widget> activeWidgets = new List<Widget>();
        public static float DPIScale;
        public HUD()
        {
            Main.GetGame().Window.ClientSizeChanged += WindowResized;
        }

        private void WindowResized(object sender, EventArgs e)
        {
            CalcDPIScale();
        }

        private void CalcDPIScale()
        {
            //DPIScale = Main.GetGame().Window.ClientBounds.Width > Main.GetGame().Window.ClientBounds.Height ? Main.GetGame().Window.ClientBounds.Width / 1920.0f : Main.GetGame().Window.ClientBounds.Height / 1080.0f;
            DPIScale = Main.GetGame().Window.ClientBounds.Height / 1080.0f;
            Logger.Log(ELogCategory.LogUI, "DPI SCALE IS: ", DPIScale);

            foreach (Widget widget in activeWidgets)
            {
                widget.UpdateScale();
            }
        }

        public unsafe void InitHUD(ref SpriteBatch spriteBatch, ref GraphicsDeviceManager gdm)
        {
            this.spriteBatch = spriteBatch;
            GDM = gdm;
            CalcDPIScale();
        }

        public static T CreateWidget<T>(params object?[]? args) where T : Widget
        {
            T newWidget = (T)Activator.CreateInstance(typeof(T), args);
            newWidget.widgetDestroyed += HUDElementDestroyed;
            newWidget.UpdateScale();
            newWidget.Construct();
            activeWidgets.Add(newWidget);
            return newWidget;
        }

        private static void HUDElementDestroyed(Events.WidgetDestroyEventArgs args)
        {
            FullDestroyWidget(args.destroyedWidget);
        }

        public static void FullDestroyWidget(Widget w)
        {
            if (w != null)
            {
                activeWidgets.Remove(w);
            }
        }

        public unsafe void DrawWidgets()
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicWrap);
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
