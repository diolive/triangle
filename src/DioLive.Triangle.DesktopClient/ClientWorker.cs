using System;
using System.Threading.Tasks;

using DioLive.Triangle.BindingModels;

using Microsoft.AspNet.SignalR.Client;

namespace DioLive.Triangle.DesktopClient
{
    internal class ClientWorker : IDisposable
    {
        private HubConnection hubConnection;
        private IHubProxy mainHubProxy;

        private Guid id;
        private byte team;

        public event Action<CurrentResponse> UpdateCurrent;
        public event Action<NeighboursResponse> UpdateNeighbours;
        public event Action<RadarResponse> UpdateRadar;
        public event Action Destroyed;

        public ClientWorker(string url)
        {
            this.hubConnection = new HubConnection(url);

            this.mainHubProxy = this.hubConnection.CreateHubProxy("main");

            this.mainHubProxy.On<CreateResponse>("OnCreate", createResponse =>
            {
                this.id = createResponse.Id;
                this.team = createResponse.Team;
            });

            this.mainHubProxy.On<CurrentResponse>("OnUpdateCurrent", r => UpdateCurrent?.Invoke(r));
            this.mainHubProxy.On<NeighboursResponse>("OnUpdateNeighbours", r => UpdateNeighbours?.Invoke(r));
            this.mainHubProxy.On<RadarResponse>("OnUpdateRadar", r => UpdateRadar?.Invoke(r));
            this.mainHubProxy.On("OnDestroyed", () => Destroyed?.Invoke());
        }

        public async Task ActivateAsync()
        {
            await this.hubConnection.Start().ConfigureAwait(false);
        }

        public async Task UpdateAsync(byte moveDirection, byte beamDirection)
        {
            await this.mainHubProxy.Invoke("Update", new UpdateRequest(this.id, moveDirection, beamDirection)).ConfigureAwait(false);
        }

        public async Task UpdateAsync(byte moveDirection)
        {
            await this.mainHubProxy.Invoke("Update", new UpdateRequest(this.id, moveDirection, default(byte?))).ConfigureAwait(false);
        }

        public void Dispose()
        {
            this.hubConnection.Dispose();
        }
    }
}
