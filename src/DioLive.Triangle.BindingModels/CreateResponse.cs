using System;
using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class CreateResponse
    {
        [JsonConstructor]
        public CreateResponse(Guid id, byte team)
        {
            Id = id;
            Team = team;
        }

        public Guid Id { get; }

        public byte Team { get; }
    }
}