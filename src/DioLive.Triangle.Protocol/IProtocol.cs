using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol
{
    public interface IProtocol
    {
        IMessageEncoder<CreateResponse> CreateResponse { get; }

        IMessageEncoder<CurrentResponse> CurrentResponse { get; }

        IMessageEncoder<NeighboursResponse> NeighboursResponse { get; }

        IMessageEncoder<RadarResponse> RadarResponse { get; }

        IMessageEncoder<UpdateRequest> UpdateRequest { get; }

        IMessageEncoder<SignoutRequest> SignoutRequest { get; }
    }
}