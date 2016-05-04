using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    public class BinaryProtocol : IProtocol
    {
        public BinaryProtocol()
        {
            this.CreateResponse = new BinaryCreateResponseMessageEncoder();
            this.CurrentResponse = new BinaryCurrentResponseMessageEncoder();
            this.NeighboursResponse = new BinaryNeighboursResponseMessageEncoder();
            this.RadarResponse = new BinaryRadarResponseMessageEncoder();
            this.UpdateRequest = new BinaryUpdateRequestMessageEncoder();
            this.SignoutRequest = new BinarySignoutRequestMessageEncoder();
        }

        public IMessageEncoder<CreateResponse> CreateResponse { get; }

        public IMessageEncoder<CurrentResponse> CurrentResponse { get; }

        public IMessageEncoder<NeighboursResponse> NeighboursResponse { get; }

        public IMessageEncoder<RadarResponse> RadarResponse { get; }

        public IMessageEncoder<UpdateRequest> UpdateRequest { get; }

        public IMessageEncoder<SignoutRequest> SignoutRequest { get; }
    }
}