using System;
using System.Net.Http;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;
using Newtonsoft.Json;

namespace DioLive.Triangle.ServerClient
{
    public class Client : IDisposable
    {
        private HttpClient httpClient;

        public Client(Uri serverUri)
        {
            this.httpClient = new HttpClient
            {
                BaseAddress = serverUri
            };

            var task = InitializeAsync();
            task.Wait();
            var result = task.Result;

            this.Id = result.Id;
            this.Team = result.Team;
        }

        public Guid Id { get; }

        public byte Team { get; }

        private async Task<CreateDotResponse> InitializeAsync()
        {
            //HttpRequestMessage message = new HttpRequestMessage(HttpMethod.Post, "create");
            HttpResponseMessage response = await httpClient.PostAsync("create", null);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Request error on InitializeAsync");
            }

            string content = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CreateDotResponse>(content);
        }

        public StateResponse GetState()
        {
            var task = GetStateAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<StateResponse> GetStateAsync()
        {
            string content = await httpClient.GetStringAsync($"state?id={this.Id}");
            return JsonConvert.DeserializeObject<StateResponse>(content);
        }

        public void Update(float angle)
        {
            var task = UpdateAsync(angle);
            task.Wait();
        }

        public async Task UpdateAsync(float angle)
        {
            await httpClient.PostAsJsonAsync("update", new UpdateRequest(this.Id, angle));
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.httpClient.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}