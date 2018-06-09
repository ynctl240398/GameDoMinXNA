using System;

namespace GameDoMinXNA
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (MineDetection game = new MineDetection())
            {
                game.Run();
            }
        }
    }
#endif
}

