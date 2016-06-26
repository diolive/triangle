using Microsoft.Xna.Framework;

namespace DioLive.Triangle.DesktopClient.Configuration
{
    public class ColorsSection
    {
        public ColorsSection(Color background, Color[] teams, Color beam)
        {
            this.Background = background;
            this.Teams = teams;
            this.Beam = beam;
        }

        public Color Background { get; }

        public Color[] Teams { get; }

        public Color Beam { get; }
    }
}