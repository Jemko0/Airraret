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
        public Widget(HUD ownerHUD, Rectangle renderTransform)
        {
            this.ownerHUD = ownerHUD;
            geometry = renderTransform;
        }

        /// <summary>
        /// Gets called before this widget is renderd for the first time,
        /// useful for initializing things before this widget is shown.
        /// </summary>
        public virtual void Construct()
        {
            throw new Exception("Default Widget cannot invoke Construct()");
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public unsafe virtual void Draw(ref SpriteBatch spriteBatch)
        {

        }

        public bool isHovered()
        {
            return geometry.Contains(Mouse.GetState().X, Mouse.GetState().Y);
        }
    }
}
