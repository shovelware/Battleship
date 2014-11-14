using System;

namespace Battleship
{
#if WINDOWS || XBOX
    static class Program
    {
        static void Main(string[] args)
        {
            using (Battleship game = new Battleship())
            {
                game.Run();
            }
        }
    }
#endif
}

