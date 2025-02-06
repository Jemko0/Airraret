using EngineZ.classes.interfaces;
using EngineZ.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace EngineZ.UI
{
    public class Widget : IUserInterface, IDisposable
    {
        protected HUD ownerHUD;
        protected Rectangle geometry;
        protected bool enabled;
        protected bool visible = false;

        public bool suppressDraw;
        public Widget(HUD ownerHUD, Rectangle renderTransform)
        {
            this.ownerHUD = ownerHUD;
            geometry = renderTransform;
            visible = true;
        }

        public void RelativizeGeometry(Widget relativeTo)
        {
            geometry.X += relativeTo.geometry.X;
            geometry.Y += relativeTo.geometry.Y;
            geometry.Width = relativeTo.geometry.Width;
            geometry.Height = relativeTo.geometry.Height;
        }

        public Rectangle GetGeometry()
        {
            return geometry;
        }

        public void SetGeometry(Rectangle newGeo)
        {
            geometry = newGeo;
        }

        public void SetEnabled(bool newState)
        {
            enabled = newState;
        }

        /// <summary>
        /// Gets called before this widget is renderd for the first time,
        /// useful for initializing things before this widget is shown.
        /// </summary>
        public virtual void Construct()
        {
            //throw new Exception("Default Widget cannot invoke Construct()");
        }

        public void DestroyWidget()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public unsafe virtual void Draw(ref SpriteBatch spriteBatch)
        {
            if (!visible) return;

            spriteBatch.GraphicsDevice.BlendState = enabled? BlendState.Opaque : BlendState.AlphaBlend;
        }

        public bool isHovered()
        {
            return geometry.Contains(Mouse.GetState().X, Mouse.GetState().Y);
        }
    }
}
