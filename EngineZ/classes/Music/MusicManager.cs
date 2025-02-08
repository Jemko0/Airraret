
using EngineZ.Utility;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineZ.Music
{
    public class MusicManager
    {
        static Dictionary<string, SoundEffect> musicTable = new Dictionary<string, SoundEffect>();

        public MusicManager() { }

        public List<SoundEffectInstance> activeSongs = new List<SoundEffectInstance>();
        public SoundEffectInstance nextSong;
        public SoundEffectInstance currentSong;
        private float fadeRate = 0.05f;
        private int fadeTick = 100;

        public void SetNext(string song)
        {
            nextSong = musicTable[song].CreateInstance();

            if (nextSong == currentSong)
            {
                return;
            }

            nextSong.Volume = 0.0f;
            nextSong.Play();
            currentSong = nextSong;
            activeSongs.Add(nextSong);

            Task.Run(async () => 
            {
                FadeIn(nextSong); 
            });

            Task.Run(async () =>
            {
                FadeOutOthers();
            });
        }

        public async Task FadeIn(SoundEffectInstance s)
        {
            while(s.Volume < 1.0f)
            {
                if(s.Volume + fadeRate > 1.0f)
                {
                    s.Volume = 1.0f;
                    break;
                }

                s.Volume += fadeRate;
                Logger.Log("VOLUME REPORT: ", s.Volume);
                await Task.Delay(fadeTick);
            }
        }

        public async Task FadeOutOthers()
        {
            foreach (var s in activeSongs)
            {
                if (s != nextSong)
                {
                    while (s.Volume > 0.0f)
                    {
                        if (s.Volume - fadeRate < 0.0f)
                        {
                            s.Volume = 0.0f;
                            break;
                        }

                        s.Volume -= fadeRate;
                        Logger.Log("VOLUME REPORT: ", s.Volume);
                        await Task.Delay(fadeTick);
                    }
                    activeSongs.Remove(s);
                }
            }


                    
        }

        public static void InitMusic()
        {
            musicTable.Add("Title", Main.GetGame().Content.Load<SoundEffect>("Sound/Music/Title"));
            musicTable.Add("Night", Main.GetGame().Content.Load<SoundEffect>("Sound/Music/Night"));
        }
    }
}
