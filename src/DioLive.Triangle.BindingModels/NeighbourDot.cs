using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class NeighbourDot
    {
        [JsonConstructor]
        public NeighbourDot(byte team, int offsetX, int offsetY, bool stunned = false, float? beaming = null)
        {
            this.Team = team;
            this.OffsetX = offsetX;
            this.OffsetY = offsetY;
            this.Stunned = stunned;
            this.Beaming = beaming;
        }

        public byte Team { get; }

        public int OffsetX { get; }

        public int OffsetY { get; }

        public bool Stunned { get; }

        public float? Beaming { get; }
    }
}