using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class RadarDot
    {
        [JsonConstructor]
        public RadarDot(byte team, byte rx, byte ry)
        {
            this.Team = team;
            this.RX = rx;
            this.RY = ry;
        }

        public byte Team { get; }

        public byte RX { get; }

        public byte RY { get; }
    }
}