using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;

namespace PresenceApp.Core.SignalR
{
    public abstract class ClientHub
    {
        protected readonly HubConnection hubConnection;
        public readonly IHubProxy hubProxy;

        public ClientHub(string url, string hub)
        {
            this.hubConnection = new HubConnection(url);
            this.hubProxy = hubConnection.CreateHubProxy(hub);
            this.startConnecionHub();
        }

        private void startConnecionHub()
        {
            this.hubConnection.Start().Wait();
        }
    }
}