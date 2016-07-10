using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class RadarDot
    {
        [JsonConstructor]
        public RadarDot(byte team, byte rx, byte ry)
        {
            Team = team;
            RX = rx;
            RY = ry;
        }

        public byte Team { get; }

        public byte RX { get; }

        public byte RY { get; }
    }
}