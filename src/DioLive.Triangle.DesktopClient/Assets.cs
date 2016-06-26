using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DioLive.Triangle.DesktopClient
{
    public static class Assets
    {
        public static Texture2D BeamTexture { get; set; }

        public static Texture2D DotTexture { get; set; }

        public static void Load(ContentManager content)
        {
            DotTexture = content.Load<Texture2D>("dot");
            BeamTexture = content.Load<Texture2D>("rounded");
        }
    }
}