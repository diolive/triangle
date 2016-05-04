using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class NeighbourDot
    {
        [JsonConstructor]
        public NeighbourDot(byte team, ushort rx, ushort ry, DotState state, byte beamDirection = default(byte))
        {
            this.Team = team;
            this.RX = rx;
            this.RY = ry;
            this.State = state;
            this.BeamDirection = beamDirection;
        }

        public byte Team { get; }

        public ushort RX { get; }

        public ushort RY { get; }

        public DotState State { get; set; }

        public byte BeamDirection { get; set; }
    }
}