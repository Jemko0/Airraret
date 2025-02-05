using EngineZ;
Main.Init();

namespace EngineZ
{
    public static class Main
    {
        static readonly Airraret game = new EngineZ.Airraret();
        public static Airraret GetGame() => game;

        public static void Init()
        {
            game.Run();
        }
    }
}
    
