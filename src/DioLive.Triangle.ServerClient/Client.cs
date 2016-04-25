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

        public Client(Uri serverUri, bool initialize = true)
        {
            this.httpClient = new HttpClient
            {
                BaseAddress = serverUri
            };

            if (initialize)
            {
                Initialize();
            }
        }

        public bool Initialized { get; private set; }

        public Guid Id { get; private set; }

        public byte Team { get; private set; }

        public void Initialize()
        {
            InitializeAsync().Wait();
        }

        public async Task InitializeAsync()
        {
            if (this.Initialized)
            {
                throw new InvalidOperationException("Client was already initialized before. Call .Signout() before");
            }

            HttpResponseMessage response = await httpClient.PostAsync("create", null);
            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException("Request error on InitializeAsync");
            }

            string content = await response.Content.ReadAsStringAsync();
            CreateDotResponse createResponse = JsonConvert.DeserializeObject<CreateDotResponse>(content);

            this.Id = createResponse.Id;
            this.Team = createResponse.Team;
            this.Initialized = true;
        }

        public StateResponse GetState()
        {
            var task = GetStateAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<StateResponse> GetStateAsync()
        {
            EnsureInitialized();
            string content = await httpClient.GetStringAsync($"state?id={this.Id}");
            return JsonConvert.DeserializeObject<StateResponse>(content);
        }

        public void Update(float angle, float? beaming = null)
        {
            UpdateAsync(angle, beaming).Wait();
        }

        public async Task UpdateAsync(float angle, float? beaming = null)
        {
            EnsureInitialized();
            await httpClient.PostAsJsonAsync("update", new UpdateRequest(this.Id, angle, beaming));
        }

        public void Signout()
        {
            SignoutAsync().Wait();
        }

        public async Task SignoutAsync()
        {
            EnsureInitialized();
            await httpClient.PostAsJsonAsync("signout", new SignoutRequest(this.Id));
            this.Initialized = false;
        }

        private void EnsureInitialized()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Client was not initialized. Call .Initialize() before.");
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Signout();
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