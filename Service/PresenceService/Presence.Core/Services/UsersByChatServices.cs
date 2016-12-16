using Presence.Core.Entities;
using Presence.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using Presence.Core.Interfaces.Repository;

namespace Presence.Core.Services
{
    public class UsersByChatServices : BaseServices<UsersByChat>, IUsersByChatServices
    {
        private readonly IUsersByChatRepository _repository;

        public UsersByChatServices(IUsersByChatRepository repository) : base(repository)
        {
            this._repository = repository;
        }

        public UsersByChat getUserByChat(string username, int chatId)
        {
            return this._repository.getUserByChat(username, chatId);
        }

        public IEnumerable<UsersByChat> getUserEnteredChats(Users user)
        {
            return this._repository.getUserEnteredChats(user);
        }

        public IEnumerable<UsersByChat> getUsersInChat(Chat chat)
        {
            return this._repository.getUsersInChat(chat);
        }

    }
}
