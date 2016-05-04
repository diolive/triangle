using System;
using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class UpdateRequest
    {
        [JsonConstructor]
        public UpdateRequest(Guid id, byte moveDirection, byte? beamDirection)
        {
            this.Id = id;
            this.MoveDirection = moveDirection;
            this.BeamDirection = beamDirection;
        }

        public Guid Id { get; }

        public byte MoveDirection { get; }

        public byte? BeamDirection { get; }
    }
}