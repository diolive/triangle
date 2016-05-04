using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    internal class BinaryNeighboursResponseMessageEncoder : BinaryMessageEncoder<NeighboursResponse>
    {
        public override NeighboursResponse Read(Stream stream)
        {
            using (var body = new StreamHelper(stream))
            {
                byte count = body.ReadByte();
                NeighbourDot[] neighbours = new NeighbourDot[count];
                for (int i = 0; i < count; i++)
                {
                    byte team = body.ReadByte();
                    ushort rx = body.ReadWord();
                    ushort ry = body.ReadWord();
                    DotState state = (DotState)body.ReadByte();
                    byte beamDirection = state.HasFlag(DotState.Beaming)
                        ? body.ReadByte()
                        : default(byte);
                    neighbours[i] = new NeighbourDot(team, rx, ry, state, beamDirection);
                }

                return new NeighboursResponse(neighbours);
            }
        }

        public override void Write(Stream stream, NeighboursResponse request)
        {
            using (var body = new StreamHelper(stream))
            {
                body.WriteByte((byte)request.Neighbours.Length);
                foreach (var neighbour in request.Neighbours)
                {
                    body.WriteByte(neighbour.Team);
                    body.WriteWord(neighbour.RX);
                    body.WriteWord(neighbour.RY);
                    body.WriteByte((byte)neighbour.State);
                    if (neighbour.State.HasFlag(DotState.Beaming))
                    {
                        body.WriteByte(neighbour.BeamDirection);
                    }
                }
            }
        }
    }
}