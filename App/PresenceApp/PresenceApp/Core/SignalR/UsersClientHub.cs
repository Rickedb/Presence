using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client;
using System.Threading.Tasks;
using PresenceApp.Core.Entities;

namespace PresenceApp.Core.SignalR
{
    public class UsersClientHub : ClientHub
    {
        protected struct Methods
        {
            public const string Login = "login";
            public const string LoginReturn = "loginAuthentication";
        }
        private const string userHub = "UserBroadcastHub";

        public UsersClientHub(string url) : base(url, userHub)
        {
        }

        public async Task login(Users user, Action<bool> callback)
        {
            this.hubProxy.On<bool>(Methods.LoginReturn, callback);
            await this.hubProxy.Invoke(Methods.Login, new object[] { user.Username, user.Password, user.Admin });
        }

    }
}