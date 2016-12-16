using Microsoft.AspNet.SignalR;
using Presence.Core.Entities;
using Presence.Core.Security;
using Presence.Core.Services;
using Presence.Core.SystemCore;
using System;
using System.Linq;

namespace Presence.Service.Communicator.BroadcastHubs
{
    public class UserBroadcastHub : Hub
    {
        public void Login(string username, string password, bool admin)
        {
            bool authentication = true;
            string returnMessage = string.Empty;
            var container = ServiceCore.getInstance().Container;

            var user = container.GetInstance<UsersServices>().Get(username);

            if(admin)
            {
                if (user == null)
                {
                    authentication = false;
                    returnMessage = "Não existe nenhum usuário administrador com este nome!";
                }
                if (Encryption.decrypt(user.Password, Encryption.SYSTEM_KEY) != password)
                {
                    authentication = false;
                    returnMessage = "Senha incorreta!";
                }
            }
            else
            {
                if(user == null)
                {
                    user = container.GetInstance<UsersServices>().Insert(new Users()
                    {
                        Username = username,
                        Admin = admin,
                        CreationDate = DateTime.Now
                    });
                }
                else
                    if (container.GetInstance<UsersByChatServices>().getUserEnteredChats(user).Where(x => x.LastInteraction > DateTime.Now.AddMinutes(-15)).ToList().Count > 0)
                    {
                        authentication = false;
                        returnMessage = "Este nome de usuário já está sendo usado!";
                    }
            }

            Clients.Caller.loginAuthentication(authentication, returnMessage);
        }

    }
}
