using System;

namespace DioLive.Triangle.CoreClient
{
    public class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            using (var game = new TriangleGame())
            {
                game.Run();
            }
        }
    }
}