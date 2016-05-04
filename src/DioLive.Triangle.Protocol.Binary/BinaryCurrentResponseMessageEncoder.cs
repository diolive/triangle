using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    internal class BinaryCurrentResponseMessageEncoder : BinaryMessageEncoder<CurrentResponse>
    {
        public override CurrentResponse Read(Stream stream)
        {
            using (var body = new StreamHelper(stream))
            {
                DotState state = (DotState)body.ReadByte();
                byte moveDirection = body.ReadByte();
                byte beamDirection = state.HasFlag(DotState.Beaming)
                    ? body.ReadByte()
                    : default(byte);

                return new CurrentResponse(state, moveDirection, beamDirection);
            }
        }

        public override void Write(Stream stream, CurrentResponse request)
        {
            using (var body = new StreamHelper(stream))
            {
                body.WriteByte((byte)request.State);
                body.WriteByte(request.MoveDirection);
                if (request.State.HasFlag(DotState.Beaming))
                {
                    body.WriteByte(request.BeamDirection);
                }
            }
        }
    }
}