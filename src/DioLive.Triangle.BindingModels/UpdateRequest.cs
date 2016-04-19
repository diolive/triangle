using System;
using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class UpdateRequest
    {
        [JsonConstructor]
        public UpdateRequest(Guid id, float moveDirection, float? beaming = null)
        {
            this.Id = id;
            this.MoveDirection = moveDirection;
            this.Beaming = beaming;
        }

        public Guid Id { get; }

        public float MoveDirection { get; }

        public float? Beaming { get; }
    }
}