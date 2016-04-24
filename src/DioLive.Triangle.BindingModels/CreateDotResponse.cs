using System;
using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class CreateDotResponse
    {
        [JsonConstructor]
        public CreateDotResponse(Guid id, byte team)
        {
            this.Id = id;
            this.Team = team;
        }

        public Guid Id { get; }

        public byte Team { get; }
    }
}