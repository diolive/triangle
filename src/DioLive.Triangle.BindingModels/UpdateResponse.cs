﻿using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class UpdateResponse
    {
        [JsonConstructor]
        public UpdateResponse(CurrentDot player, NeighbourDot[] neighbours, RadarDot[] radar)
        {
            this.Player = player;
            this.Neighbours = neighbours;
            this.Radar = radar;
        }

        public CurrentDot Player { get; }

        public NeighbourDot[] Neighbours { get; }

        public RadarDot[] Radar { get; }
    }
}