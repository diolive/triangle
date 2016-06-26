using System;
using System.Net.Http;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;
using DioLive.Triangle.Protocol;

namespace DioLive.Triangle.ServerClient
{
    public class HttpServerClient : ServerClientBase
    {
        private IProtocol protocol;

        public HttpServerClient(IProtocol protocol, Uri serverUri, bool initialize = true)
        {
            this.protocol = protocol;

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

        protected override async Task<CreateResponse> InitializeProtectedAsync()
        {
            HttpResponseMessage response = await HttpClient.PostAsync("create", null).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Request error on InitializeAsync");
            }

            return await protocol.CreateResponse.DecodeAsync(response.Content).ConfigureAwait(false);
        }

        protected override async Task<CurrentResponse> GetCurrentProtectedAsync()
        {
            HttpResponseMessage response = await HttpClient.GetAsync($"current?id={Id}");

            return await protocol.CurrentResponse.DecodeAsync(response.Content);
        }

        protected override async Task<NeighboursResponse> GetNeighboursProtectedAsync()
        {
            HttpResponseMessage response = await HttpClient.GetAsync($"neighbours?id={Id}");

            return await protocol.NeighboursResponse.DecodeAsync(response.Content);
        }

        protected override async Task<RadarResponse> GetRadarProtectedAsync()
        {
            HttpResponseMessage response = await HttpClient.GetAsync($"radar?id={Id}");

            return await protocol.RadarResponse.DecodeAsync(response.Content);
        }

        protected override async Task UpdateProtectedAsync(byte moveDirection, byte? beamDirection)
        {
            var updateRequest = new UpdateRequest(this.Id, moveDirection, beamDirection);
            await HttpClient.PostAsync("update", await protocol.UpdateRequest.EncodeAsync(updateRequest));
        }

        protected override async Task SignoutProtectedAsync()
        {
            var signoutRequest = new SignoutRequest(this.Id);
            await HttpClient.PostAsync("signout", await protocol.SignoutRequest.EncodeAsync(signoutRequest));
        }

        protected override void DisposeProtected()
        {
            base.DisposeProtected();
            this.HttpClient.Dispose();
        }
    }
}