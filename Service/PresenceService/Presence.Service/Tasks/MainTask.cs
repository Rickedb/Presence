using System;
using System.Linq;
using System.Collections.Generic;
using Presence.Core.Concurrent;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using Microsoft.Owin.Hosting;
using Microsoft.Owin;
using Presence.Service.Communicator;
using Presence.Service.Communicator.BroadcastHubs;
using Presence.Core.Entities;
using Presence.Core.Services;
using Presence.Core.Security;

[assembly: OwinStartup(typeof(Presence.Service.Tasks.MainTask))]
namespace Presence.Service.Tasks
{
    /// <summary>
    ///---------------------------------------------------------------------------------------------------------
    ///   Namespace:      Presence.Core.Tasks
    ///   Class:          MainTask
    ///   Description:    Task that initialize the others tasks and monitorate if they are alive.
    ///                   PS: Each task is a new child Thread.
    ///   Flow Order:     1- Clear emails error if it's not configured to send emails;
    ///                   2- Verify each task;
    ///                   3- Restart the when they're dead.
    ///   Author:         Henrique Dal Bello Batista                 Date: 13/10/2015
    ///   Notes:          
    ///---------------------------------------------------------------------------------------------------------
    ///   Revision History:
    ///   Name:            Date: //       Description: 
    ///---------------------------------------------------------------------------------------------------------
    /// </summary>

    public class MainTask : AbstractWorker
    {
        private static MainTask instance = null;
        private HubConnection hubConnection;

        public MainTask(int executeInterval)
            : base("MAIN_TASK", executeInterval)
        {
            TaskManager.getTaskExecutor().executeTask(this);
            WebApp.Start<SignalRStartup>("http://192.168.0.56:8080/");
            this.hubConnection = new HubConnection("http://192.168.0.56:8080/");
        }

        public override void initialize()
        {
            this.getLogger().log("*******************************************************************");
            this.getLogger().log("*********************INITIALIZING MAIN TASK************************");
            this.getLogger().log("*******************************************************************");
            this.checkFirstRun();
            this.startHub();
        }

        public override void executeTask()
        {
            try
            {
                this.watchDog();
                //this.cleanExpiredUsers();
            }
            catch (Exception ex)
            {
                this.getLogger().logError(ex.Message);
            }
        }

        /// <summary>
        /// Verifica se é a primeira vez rodando o serviço
        /// </summary>
        private void checkFirstRun()
        {
            if (this.getServiceCore().Container.GetInstance<UsersServices>().GetAll().ToList().Count == 0)
            {
                this.getLogger().log("PRIMEIRA VEZ RODANDO O SERVIÇO, PREENCHENDO COM DADOS PADRÕES...");
                this.getServiceCore().Container.GetInstance<UsersServices>().Insert(new Users()
                {
                    Username = "admin",
                    Admin = true,
                    Password = Encryption.Encrypt("admin"),
                    CreationDate = DateTime.Now
                });
            }
        }

        /// <summary>
        /// Inicia o Hub de comunicação de chat
        /// </summary>
        private void startHub()
        {
            this.getLogger().log("Iniciando chat Hub...");
            var chatHub = hubConnection.CreateHubProxy(typeof(ChatBroadcastHub).Name);
            var userHub = hubConnection.CreateHubProxy(typeof(UserBroadcastHub).Name);
            this.hubConnection.Start().Wait();
        }

        /// <summary>
        /// Verifica o estado do hub, para caso sofra disconnect, reinicie o hub
        /// </summary>
        private void watchDog()
        {
            if (this.hubConnection.State == ConnectionState.Disconnected)
            {
                this.getLogger().logWarning("HUB ESTÁ DESCONECTADO!");
                this.startHub();
            }
        }

        private void cleanExpiredUsers()
        {
            this.getLogger().log("Verificando usuários expirados...");
            var expiredUsers = this.getServiceCore().Container.GetInstance<UsersServices>().GetAll().Where(x => !x.Admin);
            this.getLogger().log("Usuários que podem expirar: " + expiredUsers.ToList().Count);
            foreach (Users user in expiredUsers)
            {
                var q = this.getServiceCore().Container.GetInstance<UsersByChatServices>().getUserEnteredChats(user);
                var enteredChats = q.Where(x => x.LastInteraction < DateTime.Now.AddDays(-1) || x.Connected == false);
                int excludedChats = 0;

                foreach (UsersByChat enteredChat in enteredChats)
                {
                    this.cleanUserByChatMessages(enteredChat);
                    this.getServiceCore().Container.GetInstance<UsersByChatServices>().Delete(new UsersByChat() { ID = enteredChat.ID });
                    excludedChats++;
                }

                if (enteredChats.Count() == excludedChats)
                {
                    this.getLogger().log("Usuário " + user.Username + " expirou, deletando usuário...");
                    this.getServiceCore().Container.GetInstance<UsersServices>().Delete( new Users() { Username = user.Username } );
                }
            }
        }

        private void cleanUserByChatMessages(UsersByChat userByChat)
        {
            foreach (Messages message in this.getServiceCore().Container.GetInstance<MessagesServices>().getMessagesFromChat(userByChat))
                this.getServiceCore().Container.GetInstance<MessagesServices>().Delete(new Messages() { ID = message.ID });
        }

        /// <summary>
        /// Método de configuração do Sistema. Primeiro método
        /// que deve ser executado para configuração e inicialização
        /// do Sistema
        /// </summary>
        /// <param name="parameters">Parâmetros do Sistema</param>
        public static void configure()
        {
            instance = new MainTask(1000);
        }

        /// <summary>
        /// Get unique instance
        /// </summary>
        /// <returns></returns>
        public static MainTask getInstance()
        {
            if (instance != null)
                return instance;

            throw new NullReferenceException("MainTask was not initialized!");
        }

    }
}
