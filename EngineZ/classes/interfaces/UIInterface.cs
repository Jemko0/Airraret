using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EngineZ.classes.interfaces
{
    public interface IUserInterface
    {
        public void Construct();
        public unsafe void Draw(ref SpriteBatch spriteBatch);
    }

    public interface IHUD
    {
        public unsafe void InitHUD(ref SpriteBatch spriteBatch, ref GraphicsDeviceManager gdm);
    }
}
