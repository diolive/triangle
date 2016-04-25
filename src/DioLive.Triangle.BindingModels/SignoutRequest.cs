using System;
using Newtonsoft.Json;

namespace DioLive.Triangle.BindingModels
{
    public class SignoutRequest
    {
        [JsonConstructor]
        public SignoutRequest(Guid id)
        {
            this.Id = id;
        }

        public Guid Id { get; }
    }
}