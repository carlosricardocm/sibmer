using System;

namespace Nave3D
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            Game3 game = new Game3();

            game.RecogeArgumentos(args);

            using (game)
            {
                game.Run();
            }
        }
    }
#endif
}

