using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Json
{
    public class JsonProtocol : IProtocol
    {
        public JsonProtocol()
        {
            this.CreateResponse = new JsonMessageEncoder<CreateResponse>();
            this.CurrentResponse = new JsonMessageEncoder<CurrentResponse>();
            this.NeighboursResponse = new JsonMessageEncoder<NeighboursResponse>();
            this.RadarResponse = new JsonMessageEncoder<RadarResponse>();
            this.UpdateRequest = new JsonMessageEncoder<UpdateRequest>();
            this.SignoutRequest = new JsonMessageEncoder<SignoutRequest>();
        }

        public IMessageEncoder<CreateResponse> CreateResponse { get; }

        public IMessageEncoder<CurrentResponse> CurrentResponse { get; }

        public IMessageEncoder<NeighboursResponse> NeighboursResponse { get; }

        public IMessageEncoder<RadarResponse> RadarResponse { get; }

        public IMessageEncoder<UpdateRequest> UpdateRequest { get; }

        public IMessageEncoder<SignoutRequest> SignoutRequest { get; }
    }
}