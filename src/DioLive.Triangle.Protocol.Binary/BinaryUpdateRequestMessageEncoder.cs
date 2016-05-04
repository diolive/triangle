using System;
using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    internal class BinaryUpdateRequestMessageEncoder : BinaryMessageEncoder<UpdateRequest>
    {
        public override UpdateRequest Read(Stream stream)
        {
            using (var body = new StreamHelper(stream))
            {
                Guid id = body.ReadGuid();
                byte moveDirection = body.ReadByte();
                bool hasBeam = body.ReadBit();
                byte? beamDirection = hasBeam ? body.ReadByte() : default(byte?);
                return new UpdateRequest(id, moveDirection, beamDirection);
            }
        }

        public override void Write(Stream stream, UpdateRequest request)
        {
            using (var body = new StreamHelper(stream))
            {
                body.WriteGuid(request.Id);
                body.WriteByte(request.MoveDirection);
                body.WriteBit(request.BeamDirection.HasValue);
                if (request.BeamDirection.HasValue)
                {
                    body.WriteByte(request.BeamDirection.Value);
                }
            }
        }
    }
}