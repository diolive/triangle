using System;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.ServerClient
{
    public abstract class ClientBase : IDisposable
    {
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

            CreateDotResponse createResponse = await InitializeProtectedAsync();

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
            return await GetStateProtectedAsync();
        }

        public async Task GetStateAsync(Action<StateResponse> action)
        {
            var state = await GetStateAsync();
            action(state);
        }

        public void Update(float angle, float? beaming = null)
        {
            UpdateAsync(angle, beaming).Wait();
        }

        public async Task UpdateAsync(float angle, float? beaming = null)
        {
            EnsureInitialized();
            await UpdateProtectedAsync(angle, beaming);
        }

        public void Signout()
        {
            SignoutAsync().Wait();
        }

        public async Task SignoutAsync()
        {
            EnsureInitialized();
            await SignoutProtectedAsync();
            this.Initialized = false;
        }

        #region protected abstract methods

        protected abstract Task<CreateDotResponse> InitializeProtectedAsync();

        protected abstract Task<StateResponse> GetStateProtectedAsync();

        protected abstract Task UpdateProtectedAsync(float angle, float? beaming);

        protected abstract Task SignoutProtectedAsync();

        #endregion protected abstract methods

        private void EnsureInitialized()
        {
            if (!Initialized)
            {
                throw new InvalidOperationException("Client was not initialized. Call .Initialize() before.");
            }
        }

        #region IDisposable implementation

        private bool disposed = false; // To detect redundant calls

        public void Dispose()
        {
            if (disposed)
            {
                return;
            }

            DisposeProtected();

            disposed = true;
        }

        protected virtual void DisposeProtected()
        {
            Signout();
        }

        #endregion IDisposable implementation
    }
}