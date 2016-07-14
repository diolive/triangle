using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.DesktopClient.Configuration;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DioLive.Triangle.DesktopClient.GameObjects
{
    public class Neighbourhood : GameObjectBase
    {
        private readonly Vector2 topLeft;
        private readonly float scale;
        private readonly Point dotSize;
        private readonly Vector2 dotOffset;
        private readonly Point beamSize;
        private readonly Vector2 beamOffset;

        private readonly IConfiguration configuration;

        public Neighbourhood(IConfiguration configuration)
        {
            this.configuration = configuration;

            this.topLeft = Vector2.Zero;
            Bounds = new Rectangle(this.topLeft.ToPoint(), new Point(Constants.UI.NeighbourhoodSize));
            this.dotSize = new Point(2 * Constants.UI.DotRadius * Constants.UI.NeighbourhoodSize / Constants.Space.Scope);
            this.dotOffset = this.dotSize.ToVector2() / 2f;
            this.beamSize = new Point(Constants.UI.BeamLength * Constants.UI.NeighbourhoodSize / Constants.Space.Scope, Constants.UI.BeamWidth * Constants.UI.NeighbourhoodSize / Constants.Space.Scope);
            this.beamOffset = new Vector2(0, Assets.BeamTexture.Height) / 2f;
            this.scale = (float)Constants.UI.NeighbourhoodSize / ushort.MaxValue;
        }

        public NeighboursResponse CurrentResponse { get; set; }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            if (CurrentResponse == null)
            {
                return;
            }

            foreach (var dot in CurrentResponse.Neighbours)
            {
                if (dot.State.HasFlag(DotState.Beaming))
                {
                    DrawBeam(spriteBatch, dot.RX, dot.RY, dot.BeamDirection);
                }

                DrawDot(spriteBatch, dot);
            }
        }

        private void DrawBeam(SpriteBatch spriteBatch, ushort rx, ushort ry, byte direction)
        {
            Vector2 beamStart = ToNeighbourhoodVector(rx, ry);
            Rectangle beamRect = new Rectangle(beamStart.ToPoint(), this.beamSize);
            spriteBatch.Draw(
                texture: Assets.BeamTexture,
                destinationRectangle: beamRect,
                origin: this.beamOffset,
                rotation: (float)AngleHelper.DirectionToRadians(direction),
                color: this.configuration.Colors.Beam);
        }

        private void DrawDot(SpriteBatch spriteBatch, NeighbourDot dot)
        {
            Vector2 dotCenter = ToNeighbourhoodVector(dot.RX, dot.RY);
            Rectangle dotRect = new Rectangle((dotCenter - this.dotOffset).ToPoint(), this.dotSize);
            spriteBatch.Draw(Assets.DotTexture, dotRect, this.configuration.Colors.Teams[dot.Team]);
        }

        private Vector2 ToNeighbourhoodVector(ushort rx, ushort ry)
        {
            return (new Vector2(rx, ry) * this.scale) + this.topLeft;
        }
    }
}