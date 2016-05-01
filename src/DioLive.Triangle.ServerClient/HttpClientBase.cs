using System;
using System.Net.Http;

namespace DioLive.Triangle.ServerClient
{
    public abstract class HttpClientBase : ClientBase
    {
        public HttpClientBase(Uri serverUri, bool initialize = true)
        {
            this.HttpClient = new HttpClient
            {
                BaseAddress = serverUri
            };

            if (initialize)
            {
                Initialize();
            }
        }

        protected HttpClient HttpClient { get; }

        protected override void DisposeProtected()
        {
            base.DisposeProtected();
            this.HttpClient.Dispose();
        }
    }
}