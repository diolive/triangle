using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class NeighboursResponse
    {
        static NeighboursResponse()
        {
            None = new NeighboursResponse(new NeighbourDot[0]);
        }

        public static NeighboursResponse None { get; }

        [JsonConstructor]
        public NeighboursResponse(NeighbourDot[] neighbours)
        {
            this.Neighbours = neighbours;
        }

        public NeighbourDot[] Neighbours { get; }
    }
}