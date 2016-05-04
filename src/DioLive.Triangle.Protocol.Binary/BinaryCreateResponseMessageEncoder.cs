using System;
using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    internal class BinaryCreateResponseMessageEncoder : BinaryMessageEncoder<CreateResponse>
    {
        public override CreateResponse Read(Stream stream)
        {
            using (var body = new StreamHelper(stream))
            {
                Guid id = body.ReadGuid();
                byte team = body.ReadByte();

                return new CreateResponse(id, team);
            }
        }

        public override void Write(Stream stream, CreateResponse request)
        {
            using (var body = new StreamHelper(stream))
            {
                body.WriteGuid(request.Id);
                body.WriteByte(request.Team);
            }
        }
    }
}