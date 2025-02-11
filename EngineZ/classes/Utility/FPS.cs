using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EngineZ
{
    public class FPSCounter : GameComponent
    {
        private double frames = 0;
        private double updates = 0;
        private double elapsed = 0;
        private double last = 0;
        private double now = 0;
        public double msgFrequency = 1.0f;
        public string fps = "-1";
        public string msg = "";

        public FPSCounter(Game game) : base(game)
        {
        }

        /// <summary>
        /// The msgFrequency here is the reporting time to update the message.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            now = gameTime.TotalGameTime.TotalSeconds;
            elapsed = (double)(now - last);
            if (elapsed > msgFrequency)
            {
                fps = (frames / elapsed).ToString();
                elapsed = 0;
                frames = 0;
                updates = 0;
                last = now;
            }
            updates++;
        }
    }
}
