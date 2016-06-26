using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DesktopClient.Configuration;
using DioLive.Triangle.ServerClient;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DioLive.Triangle.DesktopClient.GameObjects
{
    public class Radar : GameObjectBase
    {
        private Rectangle bounds;
        private Vector2 topLeft;
        private float scale;
        private Point dotSize;
        private Vector2 dotOffset;

        private RadarResponse currentResponse;
        private ManualTimer updateTimer;
        private IConfiguration configuration;
        private ServerClientBase client;

        public Radar(IConfiguration configuration, ServerClientBase client)
        {
            this.configuration = configuration;
            this.client = client;

            this.topLeft = new Vector2(Constants.UI.NeighbourhoodSize, 0);
            this.bounds = new Rectangle(this.topLeft.ToPoint(), new Point(Constants.UI.RadarSize));
            this.dotSize = new Point(Constants.UI.RadarDotRadius);
            this.dotOffset = this.dotSize.ToVector2() / 2f;
            this.scale = (float)Constants.UI.RadarSize / byte.MaxValue;

            this.updateTimer = new ManualTimer(Constants.Timers.GetRadarInterval);
            this.updateTimer.Tick += (s, e) => { this.currentResponse = this.client.GetRadar(); };
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            this.updateTimer += gameTime.ElapsedGameTime;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (this.currentResponse == null)
            {
                return;
            }

            foreach (var dot in this.currentResponse.RadarDots)
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