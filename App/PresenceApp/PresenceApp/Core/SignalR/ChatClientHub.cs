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
    public class ChatClientHub : ClientHub
    {
        public struct Methods
        {
            public const string SendMessage = "sendMessage";
            public const string ReceiveMessage = "sendChatMessage";

            public const string EnterChat = "enterChat";
            public const string LeaveChat = "leaveChat";
            public const string EnterChatConfirmation = "chatEntered";

            public const string RefreshOnlineUsers = "refreshOnlineUsers";
            public const string refreshOnlineUsersCallback = "refreshOnlineUsersList";

            public const string ChatStatus = "chatStatus";
            public const string RefreshChats = "refreshChats";
            public const string AvailableChats = "getAvailableChats";

            public const string GetLastMessages = "getLastMessages";
            public const string ReceiveLastMessages = "lastMessages";

            public const string KickUser = "kickUser";
            public const string KickedUser = "kickedUser";

            public const string CreateChat = "createChat";
            public const string ChatCreated = "chatCreated";

            public const string CloseChat = "closeChat";
        }
        private const string chatHub = "ChatBroadcastHub";


        public ChatClientHub(string url) : base(url, chatHub)
        {

        }

        public void ConfigureCallBacks(Action<int, string, string> receiveMessage,
                                        Action<object> refreshOnlineUsers, Action<int> chatStatus, Action<object> lastMessages, 
                                        Action<object> getAvailableChats)
        {
            this.hubProxy.On(Methods.ReceiveMessage, receiveMessage);
            
            this.hubProxy.On(Methods.ChatStatus, chatStatus);
            this.hubProxy.On(Methods.AvailableChats, getAvailableChats);
            this.hubProxy.On(Methods.ReceiveLastMessages, lastMessages);
        }

        public void ConfigureCallBacks(Action<int, string> kickedUser)
        {
            this.hubProxy.On(Methods.KickedUser, kickedUser);
        }

        public void ConfigureCallBacks(Action<int, string, string> receiveMessage, Action<int, string> kickedUser)
        {
            this.hubProxy.On(Methods.ReceiveMessage, receiveMessage);
            this.hubProxy.On(Methods.KickedUser, kickedUser);
        }

        public void ConfigureCallBacks(Action<object> getAvailableChats)
        {
            this.hubProxy.On(Methods.AvailableChats, getAvailableChats);
        }

        public void enterChat(Action<bool> enterChatConfirmation, int chatId, string user)
        {
            this.hubProxy.On(Methods.EnterChatConfirmation, enterChatConfirmation);
            this.hubProxy.Invoke(Methods.EnterChat, new object[] { chatId, user });
        }

        public void leaveChat(int chatId, string user)
        {
            this.hubProxy.Invoke(Methods.LeaveChat, chatId, user);
        }

        public void GetLast50Messages(Action<object> lastMessages, int chatId)
        {
            this.hubProxy.On(Methods.ReceiveLastMessages, lastMessages);
            this.hubProxy.Invoke(Methods.GetLastMessages, chatId);
        }

        public async Task sendMessage(int chatId, string user, string msg)
        {
            await this.hubProxy.Invoke(Methods.SendMessage, new object[] { chatId, user, msg });
        }

        public void refreshChats()
        {
            this.hubProxy.Invoke(Methods.RefreshChats);
        }

        public void refreshUsers(Action<object> refreshCallback, int chatId)
        {
            this.hubProxy.On(Methods.refreshOnlineUsersCallback, refreshCallback);
            this.hubProxy.Invoke(Methods.RefreshOnlineUsers, chatId);
        }

        public void kickUser(string username, int chatId, string adminUsername)
        {
            this.hubProxy.Invoke(Methods.KickUser, username, chatId, adminUsername);
        }

        public void CreateChat(Action<bool> creationConfirmation, string creator, string chatName, int capacity)
        {
            this.hubProxy.On(Methods.ChatCreated, creationConfirmation);
            this.hubProxy.Invoke(Methods.CreateChat, creator, chatName, capacity);
        }

        public void closeChat(int chatId)
        {
            this.hubProxy.Invoke(Methods.CloseChat, chatId);
        }
    }
}