using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DesktopClient.Configuration;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DioLive.Triangle.DesktopClient.GameObjects
{
    public class Radar : GameObjectBase
    {
        private readonly Rectangle bounds;
        private readonly Vector2 topLeft;
        private readonly float scale;
        private readonly Point dotSize;
        private readonly Vector2 dotOffset;

        private readonly IConfiguration configuration;

        public Radar(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.topLeft = new Vector2(Constants.UI.NeighbourhoodSize, 0);
            this.bounds = new Rectangle(this.topLeft.ToPoint(), new Point(Constants.UI.RadarSize));
            this.dotSize = new Point(Constants.UI.RadarDotRadius);
            this.dotOffset = this.dotSize.ToVector2() / 2f;
            this.scale = (float)Constants.UI.RadarSize / byte.MaxValue;
        }

        public RadarResponse CurrentResponse { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (this.CurrentResponse == null)
            {
                return;
            }

            foreach (var dot in this.CurrentResponse.RadarDots)
            {
                this.DrawRadarPoint(spriteBatch, dot.RX, dot.RY, dot.Team);
            }
        }

        private void DrawRadarPoint(SpriteBatch spriteBatch, byte rx, byte ry, int team)
        {
            Vector2 dotCenter = this.ToRadarVector(rx, ry);
            Rectangle dotRect = new Rectangle((dotCenter - this.dotOffset).ToPoint(), this.dotSize);
            spriteBatch.Draw(Assets.DotTexture, dotRect, this.configuration.Colors.Teams[team]);
        }

        private Vector2 ToRadarVector(byte rx, byte ry)
        {
            return (new Vector2(rx, ry) * this.scale) + this.topLeft;
        }
    }
}