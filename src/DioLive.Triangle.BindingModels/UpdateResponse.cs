using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class UpdateResponse
    {
        [JsonConstructor]
        public UpdateResponse(CurrentState player, NeighbourDot[] neighbours, RadarDot[] radar)
        {
            this.Player = player;
            this.Neighbours = neighbours;
            this.Radar = radar;
        }

        public CurrentState Player { get; }

        public NeighbourDot[] Neighbours { get; }

        public RadarDot[] Radar { get; }
    }
}