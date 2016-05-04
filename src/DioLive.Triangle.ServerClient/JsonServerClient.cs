using System;
using DioLive.Triangle.Protocol;
using DioLive.Triangle.Protocol.Json;

namespace DioLive.Triangle.ServerClient
{
    public class JsonServerClient : HttpServerClient
    {
        private static IProtocol protocol;

        static JsonServerClient()
        {
            protocol = new JsonProtocol();
        }

        public JsonServerClient(Uri serverUri, bool initialize = true)
            : base(protocol, serverUri, initialize)
        {
        }
    }
}