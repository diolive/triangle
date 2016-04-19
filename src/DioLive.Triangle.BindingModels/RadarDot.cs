using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class RadarDot
    {
        [JsonConstructor]
        public RadarDot(byte team, int offsetX, int offsetY)
        {
            this.Team = team;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
        }

        public byte Team { get; }

        public int OffsetX { get; }

        public int OffsetY { get; }
    }
}