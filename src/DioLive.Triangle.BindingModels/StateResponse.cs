using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class StateResponse
    {
        static StateResponse()
        {
            Destroyed = new StateResponse(CurrentDot.Destroyed, null, null);
        }

        public static StateResponse Destroyed { get; }

        [JsonConstructor]
        public StateResponse(CurrentDot current, NeighbourDot[] neighbours, RadarDot[] radar)
        {
            this.Current = current;
            this.Neighbours = neighbours;
            this.Radar = radar;
        }

        public CurrentDot Current { get; }

        public NeighbourDot[] Neighbours { get; }

        public RadarDot[] Radar { get; }
    }
}