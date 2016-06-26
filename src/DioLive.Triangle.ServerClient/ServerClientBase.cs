using System;
using System.Threading.Tasks;
using DioLive.Triangle.BindingModels;

namespace DioLive.Triangle.ServerClient
{
    public abstract class ServerClientBase : IDisposable
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

            CreateResponse createResponse = await InitializeProtectedAsync().ConfigureAwait(false);

            this.Id = createResponse.Id;
            this.Team = createResponse.Team;
            this.Initialized = true;
        }

        public CurrentResponse GetCurrent()
        {
            var task = GetCurrentAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<CurrentResponse> GetCurrentAsync()
        {
            EnsureInitialized();
            return await GetCurrentProtectedAsync().ConfigureAwait(false);
        }

        public async Task GetCurrentAsync(Action<CurrentResponse> action)
        {
            var state = await GetCurrentAsync().ConfigureAwait(false);
            action(state);
        }

        public NeighboursResponse GetNeighbours()
        {
            var task = GetNeighboursAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<NeighboursResponse> GetNeighboursAsync()
        {
            EnsureInitialized();
            return await GetNeighboursProtectedAsync().ConfigureAwait(false);
        }

        public async Task GetNeighboursAsync(Action<NeighboursResponse> action)
        {
            var state = await GetNeighboursAsync().ConfigureAwait(false);
            action(state);
        }

        public RadarResponse GetRadar()
        {
            var task = GetRadarAsync();
            task.Wait();
            return task.Result;
        }

        public async Task<RadarResponse> GetRadarAsync()
        {
            EnsureInitialized();
            return await GetRadarProtectedAsync().ConfigureAwait(false);
        }

        public async Task GetRadarAsync(Action<RadarResponse> action)
        {
            var state = await GetRadarAsync().ConfigureAwait(false);
            action(state);
        }

        public void Update(byte moveDirection, byte? beamDirection = default(byte?))
        {
            UpdateAsync(moveDirection, beamDirection).Wait();
        }

        public async Task UpdateAsync(byte moveDirection, byte? beamDirection = default(byte?))
        {
            EnsureInitialized();
            await UpdateProtectedAsync(moveDirection, beamDirection).ConfigureAwait(false);
        }

        public void Signout()
        {
            SignoutAsync().Wait();
        }

        public async Task SignoutAsync()
        {
            EnsureInitialized();
            await SignoutProtectedAsync().ConfigureAwait(false);
            this.Initialized = false;
        }

        #region protected abstract methods

        protected abstract Task<CreateResponse> InitializeProtectedAsync();

        protected abstract Task<CurrentResponse> GetCurrentProtectedAsync();

        protected abstract Task<NeighboursResponse> GetNeighboursProtectedAsync();

        protected abstract Task<RadarResponse> GetRadarProtectedAsync();

        protected abstract Task UpdateProtectedAsync(byte moveDirection, byte? beamDirection);

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