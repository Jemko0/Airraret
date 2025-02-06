using EngineZ.classes.interfaces;
using EngineZ.DataStructures;
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
        private Rectangle geometry;
        protected Rectangle scaledGeometry;
        protected bool enabled;
        protected bool visible = false;
        public EWidgetAlignment alignment;
        public Vector2 origin;
        public bool isHovered => scaledGeometry.Contains(Mouse.GetState().X, Mouse.GetState().Y);

        public bool suppressDraw;
        public Widget(HUD ownerHUD, Rectangle renderTransform)
        {
            this.ownerHUD = ownerHUD;
            geometry = renderTransform;
            visible = true;
        }

        public Widget(HUD ownerHUD, Rectangle renderTransform, EWidgetAlignment widgetAlignment)
        {
            this.ownerHUD = ownerHUD;
            geometry = renderTransform;
            alignment = widgetAlignment;
            visible = true;
        }

        /// <summary>
        /// Updates the widgets position based on DPI Scale
        /// </summary>
        public void UpdateScale()
        {
            switch(alignment)
            {
                case EWidgetAlignment.TopLeft:
                    
                    scaledGeometry.X = (int)(origin.X - geometry.X * HUD.DPIScale);
                    scaledGeometry.Y = (int)(origin.Y * HUD.DPIScale - geometry.Y);
                    
                    scaledGeometry.Width = (int)(geometry.Width * HUD.DPIScale);
                    scaledGeometry.Height = (int)(geometry.Height * HUD.DPIScale);
                    break;

                case EWidgetAlignment.Fill:
                    scaledGeometry.X = geometry.X;
                    scaledGeometry.Y = geometry.Y;
                    scaledGeometry.Width = (int)(Main.GetGame().Window.ClientBounds.Width - geometry.Width);
                    scaledGeometry.Height = (int)(Main.GetGame().Window.ClientBounds.Height - geometry.Height);
                    break;
            }
        }

        /// <summary>
        /// makes the widgets geometry relative to another widgets geometry
        /// </summary>
        /// <param name="relativeTo"></param>
        /// <param name="inheritWH">set to false to handle width and height manually</param>
        public void RelativizeGeometry(Widget relativeTo, bool inheritWH = true)
        {
            geometry.X = relativeTo.geometry.X;
            geometry.Y = relativeTo.geometry.Y;
            origin = relativeTo.origin;

            if(inheritWH)
            {
                geometry.Width = relativeTo.geometry.Width;
                geometry.Height = relativeTo.geometry.Height;
            }
        }

        public Rectangle GetGeometry()
        {
            return geometry;
        }

        /// <summary>
        /// Sets the widgets geometry and updates its DPI Scale automatically
        /// </summary>
        /// <param name="newGeo"></param>
        public void SetGeometry(Rectangle newGeo)
        {
            geometry = newGeo;
            UpdateScale();
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

        /// <summary>
        /// Removes this widget from any parent that it might have and clears it from memory
        /// </summary>
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
    }
}
