using System;
using System.IO;
using DioLive.Common.Helpers;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.Protocol.Binary
{
    internal class BinarySignoutRequestMessageEncoder : BinaryMessageEncoder<SignoutRequest>
    {
        public override SignoutRequest Read(Stream stream)
        {
            using (var body = new StreamHelper(stream))
            {
                Guid id = body.ReadGuid();
                return new SignoutRequest(id);
            }
        }

        public override void Write(Stream stream, SignoutRequest request)
        {
            using (var body = new StreamHelper(stream))
            {
                body.WriteGuid(request.Id);
            }
        }
    }
}