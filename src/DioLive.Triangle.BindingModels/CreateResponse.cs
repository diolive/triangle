using System;
using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class CreateResponse
    {
        [JsonConstructor]
        public CreateResponse(Guid id, byte team)
        {
            this.Id = id;
            this.Team = team;
        }

        public Guid Id { get; }

        public byte Team { get; }
    }
}