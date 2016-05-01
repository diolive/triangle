using System;
using System.Net.Http;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;
using Newtonsoft.Json;

namespace DioLive.Triangle.ServerClient
{
    public class JsonClient : HttpClientBase
    {
        public JsonClient(Uri serverUri, bool initialize = true)
            : base(serverUri, initialize)
        {
        }

        protected override async Task<CreateDotResponse> InitializeProtectedAsync()
        {
            HttpResponseMessage response = await HttpClient.PostAsync("create", null);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Request error on InitializeAsync");
            }

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateDotResponse>(content);
        }

        protected override async Task<StateResponse> GetStateProtectedAsync()
        {
            string content = await HttpClient.GetStringAsync($"state?id={Id}");
            return JsonConvert.DeserializeObject<StateResponse>(content);
        }

        protected override async Task UpdateProtectedAsync(float angle, float? beaming)
        {
            await HttpClient.PostAsJsonAsync("update", new UpdateRequest(this.Id, angle, beaming));
        }

        protected override async Task SignoutProtectedAsync()
        {
            await HttpClient.PostAsJsonAsync("signout", new SignoutRequest(this.Id));
        }
    }
}