using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DioLive.Triangle.DesktopClient
{
    public static class Assets
    {
        private static ContentManager Content;

        public static void Load(ContentManager content)
        {
            Content = content;
        }

        public static class Textures
        {
            public static Texture2D Beam { get; set; }

            public static Texture2D Dot { get; set; }

            static Textures()
            {
                Dot = Content.Load<Texture2D>("dot");
                Beam = Content.Load<Texture2D>("rounded");
            }
        }

    }
}