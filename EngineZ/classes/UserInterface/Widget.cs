using EngineZ.classes.interfaces;
using EngineZ.DataStructures;
using EngineZ.Events;
using EngineZ.Input;
using EngineZ.Utility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Runtime.CompilerServices;

namespace EngineZ.UI
{
    public class Widget : IUserInterface, IDisposable
    {
        protected HUD ownerHUD;
        private Rectangle geometry;
        protected Rectangle scaledGeometry;
        protected bool enabled;
        public bool visible = false;
        public EWidgetAlignment alignment;
        public Vector2 origin;
#if DEBUG
        public bool widgetDebugDraw = false;
#endif
        public bool isHovered => scaledGeometry.Contains(Mouse.GetState().X, Mouse.GetState().Y);

        public bool suppressDraw;

        public delegate void OnWidgetDestroy(WidgetDestroyEventArgs args);
        public event OnWidgetDestroy widgetDestroyed;

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

        public ref Rectangle GetGeometry()
        {
            return ref geometry;
        }

        public Rectangle GetScaledGeometry()
        {
            return scaledGeometry;
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
            visible = false;
            suppressDraw = true;
            Dispose();
        }

        private bool disposed = false;
        public void Dispose()
        {
            Dispose(true);
            //GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    ownerHUD = null;
                    visible = false;
                    ClearDelegates();
                    if(widgetDestroyed != null)
                    {
                        widgetDestroyed.Invoke(new WidgetDestroyEventArgs(this));
                    }
                    else
                    {
                        Logger.Log(ELogCategory.LogUI, "widgetDestroyed delegate wasw null!");
                    }
                    
                }

                disposed = true;
            }
        }

        /// <summary>
        /// use to remove any custom delegates on widget destroyation :p
        /// </summary>
        public virtual void ClearDelegates()
        {

        }
        private Texture2D debugRect;
        private Texture2D debugRectOutline;
        public unsafe virtual void Draw(ref SpriteBatch spriteBatch)
        {
            if (!visible) return;

            spriteBatch.GraphicsDevice.BlendState = enabled? BlendState.Opaque : BlendState.AlphaBlend;
#if DEBUG
            if (widgetDebugDraw)
            {
                if (debugRect != null)
                {
                    spriteBatch.Draw(debugRectOutline, scaledGeometry, Color.Red);
                    spriteBatch.Draw(debugRect, new Rectangle((int)origin.X, (int)origin.Y, 8, 8), Color.Yellow);
                    spriteBatch.Draw(debugRect, new Rectangle(scaledGeometry.X, scaledGeometry.Y, 4, 4), Color.Blue);
                    spriteBatch.DrawString(Airraret.gameFont24, GetType().Name, new Vector2(scaledGeometry.X, scaledGeometry.Y), Color.Yellow);
                }
                else
                {
                    debugRect = new Texture2D(Main.GetGame().GraphicsDevice, 1, 1);
                    debugRect.SetData(new Color[] { Color.White });
                    debugRectOutline = Main.GetGame().Content.Load<Texture2D>("Textures/UI/RectOutline");
                }
                
            }
#endif
        }
    }
}
