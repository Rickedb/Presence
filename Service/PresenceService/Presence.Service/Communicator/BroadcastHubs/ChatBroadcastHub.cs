using Microsoft.AspNet.SignalR;
using Presence.Core.Entities;
using Presence.Core.Services;
using Presence.Core.SystemCore;
using System;
using System.Linq;

namespace Presence.Service.Communicator.BroadcastHubs
{
    public class ChatBroadcastHub : Hub
    {
        public void SendMessage(int chatId, string user, string msg)
        {
            try
            {
                var userByChat = ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().getUserByChat(user, chatId);
                this.userInteraction(userByChat);

                var message = new Messages()
                {
                    UsersByChatID = userByChat.ID,
                    Message = msg,
                    Type = Messages.Types.STRING,
                    InsertedDate = DateTime.Now
                };

                ServiceCore.getInstance().Container.GetInstance<MessagesServices>().Insert(message);
            }
            catch (Exception ex)
            {
                //log
            }

            Clients.All.sendChatMessage(chatId, user, msg);
        }

        public void EnterChat(int chatId, string username)
        {
            var userByChat = ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().GetAll().Where(x => x.ID_Chat == chatId);

            var test = userByChat.Where(x => x.Username == username).FirstOrDefault();

            if (test != null)
                this.userInteraction(test);
            else
                ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().Insert(
                    new UsersByChat()
                    {
                        ID_Chat = chatId,
                        Username = username,
                        Users = new Users() { Username = username },
                        Connected = true,
                        LastInteraction = DateTime.Now,
                        EntryDate = DateTime.Now
                    });

            Clients.Caller.chatEntered(true);
            Clients.All.sendChatMessage(chatId, username + " entrou no chat...", string.Empty);
        }

        public void LeaveChat(int chatId, string username)
        {
            var userByChat = ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().getUsersInChat(
                new Chat()
                {
                    ID = chatId
                }).Where(x => x.Username == username).FirstOrDefault();

            if (userByChat != null)
            {
                userByChat.Connected = false;
                ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().Update(userByChat);
            }

            Clients.All.sendChatMessage(chatId, username + " saiu do chat...", string.Empty);
        }

        public void RefreshOnlineUsers(int chatId)
        {
            var users = ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().getUsersInChat(new Chat() { ID = chatId }).Where(x => x.Connected);

            Clients.Caller.refreshOnlineUsersList(users);
        }

        /// <summary>
        /// Responde com o Status atual do chat para o caller do método
        /// </summary>
        /// <param name="chatId"></param>
        public void ChatStatus(Int64 chatId)
        {
            var ended = ServiceCore.getInstance().Container.GetInstance<ChatServices>().Get(chatId).EndDate;
            Clients.Caller.chatStatus((ended == null) ? (int)Chat.Status.ACTIVE : (int)Chat.Status.NOT_ACTIVE);
        }

        public void RefreshChats()
        {
            Clients.Caller.getAvailableChats(ServiceCore.getInstance().Container.GetInstance<ChatServices>().getAvailableChats().ToArray());
        }

        public void getLastMessages(int chatId)
        {
            var messages = ServiceCore.getInstance().Container.GetInstance<MessagesServices>().getLastFifty(new Chat() { ID = chatId });
            Clients.Caller.lastMessages(messages.OrderBy(x => x.ID));
        }

        public void kickUser(string username, int chatId, string adminUsername)
        {
            if (username == adminUsername)
                return;

            var userByChat = ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().getUserByChat(username, chatId);
            foreach (Messages message in ServiceCore.getInstance().Container.GetInstance<MessagesServices>().getMessagesFromChat(userByChat))
                ServiceCore.getInstance().Container.GetInstance<MessagesServices>().Delete(new Messages() { ID = message.ID });

            ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().Delete(new UsersByChat() { ID = userByChat.ID });

            Clients.All.sendChatMessage(chatId, username + " foi expulso por " + adminUsername, string.Empty);
            Clients.All.kickedUser(chatId, username);
        }

        public void createChat(string creator, string chatName, int capacity)
        {
            var chatService = ServiceCore.getInstance().Container.GetInstance<ChatServices>();
            var chat = chatService.GetAll().Where(x => x.Name == chatName);
            bool created = false;

            if (chat.Count() == 0)
            {
                chatService.Insert(new Chat()
                {
                    ID = chatService.GetAll().OrderBy(x => x.ID).Last().ID + 1,
                    Name = chatName,
                    CreatorUsername = creator,
                    CreationDate = DateTime.Now,
                    Capacity = capacity
                });
                created = true;
            }

            Clients.Caller.chatCreated(created);
        }

        public void closeChat(int chatId)
        {
            UsersByChatServices UbCservice = ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>();
            var usersByChat = UbCservice.getUsersInChat(new Chat() { ID = chatId });

            var chatService = ServiceCore.getInstance().Container.GetInstance<ChatServices>();
            var chat = chatService.Get(chatId);
            chat.EndDate = DateTime.Now;
            chatService.Update(chat);

            foreach (UsersByChat userByChat in usersByChat)
                Clients.All.kickedUser(chatId, userByChat.Username);
        }

        private void userInteraction(UsersByChat userByChat)
        {
            userByChat.LastInteraction = DateTime.Now;
            userByChat.Connected = true;
            ServiceCore.getInstance().Container.GetInstance<UsersByChatServices>().Update(userByChat);
        }

    }
}
