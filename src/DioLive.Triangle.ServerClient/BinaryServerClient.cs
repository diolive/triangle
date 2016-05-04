using System;
using DioLive.Triangle.Protocol;
using DioLive.Triangle.Protocol.Binary;

namespace DioLive.Triangle.ServerClient
{
    public class BinaryServerClient : HttpServerClient
    {
        private static IProtocol protocol;

        static BinaryServerClient()
        {
            protocol = new BinaryProtocol();
        }

        public BinaryServerClient(Uri serverUri, bool initialize = true)
            : base(protocol, serverUri, initialize)
        {
        }
    }
}