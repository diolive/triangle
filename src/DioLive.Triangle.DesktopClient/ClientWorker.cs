using System;
using System.Threading.Tasks;

using DioLive.Triangle.BindingModels;

using Microsoft.AspNet.SignalR.Client;

namespace DioLive.Triangle.DesktopClient
{
    internal class ClientWorker : IDisposable
    {
        private HubConnection hubConnection;
        private Guid id;
        private IHubProxy mainHubProxy;
        private byte team;

        public ClientWorker(string url)
        {
            this.hubConnection = new HubConnection(url);

            this.mainHubProxy = this.hubConnection.CreateHubProxy("main");

            this.mainHubProxy.On<CreateResponse>("OnCreate", OnCreate);
            this.mainHubProxy.On<CurrentResponse>("OnUpdateCurrent", OnUpdateCurrent);
            this.mainHubProxy.On<NeighboursResponse>("OnUpdateNeighbours", OnUpdateNeighbours);
            this.mainHubProxy.On<RadarResponse>("OnUpdateRadar", OnUpdateRadar);
            this.mainHubProxy.On("OnDestroyed", OnDestroyed);
        }

        public event Action Destroyed;

        public event Action<CurrentResponse> UpdateCurrent;

        public event Action<NeighboursResponse> UpdateNeighbours;

        public event Action<RadarResponse> UpdateRadar;

        public async Task ActivateAsync()
        {
            await this.hubConnection.Start().ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.hubConnection.Dispose();
        }

        public async Task UpdateAsync(byte moveDirection, byte beamDirection)
        {
            await this.mainHubProxy.Invoke("Update", new UpdateRequest(this.id, moveDirection, beamDirection)).ConfigureAwait(false);
        }

        public async Task UpdateAsync(byte moveDirection)
        {
            await this.mainHubProxy.Invoke("Update", new UpdateRequest(this.id, moveDirection, default(byte?))).ConfigureAwait(false);
        }

        private void OnCreate(CreateResponse createResponse)
        {
            this.id = createResponse.Id;
            this.team = createResponse.Team;
        }

        private void OnDestroyed()
        {
            Destroyed?.Invoke();
        }

        private void OnUpdateCurrent(CurrentResponse currentResponse)
        {
            UpdateCurrent?.Invoke(currentResponse);
        }

        private void OnUpdateNeighbours(NeighboursResponse neighboursResponse)
        {
            UpdateNeighbours?.Invoke(neighboursResponse);
        }

        private void OnUpdateRadar(RadarResponse radarResponse)
        {
            UpdateRadar?.Invoke(radarResponse);
        }
    }
}