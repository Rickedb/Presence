using System.Collections.Generic;
using Presence.Core.Entities;
using Presence.Core.Interfaces.Services;
using Presence.Core.Interfaces.Repository;
using System;

namespace Presence.Core.Services
{
    public class MessagesServices : BaseServices<Messages>, IMessagesServices
    {
        private readonly IMessagesRepository _repository;

        public MessagesServices(IMessagesRepository repository) : base(repository)
        {
            this._repository = repository;
        }

        public IEnumerable<Messages> getLastFifty(Chat chat)
        {
            return this._repository.getLastFifty(chat);
        }

        public IEnumerable<Messages> getMessagesFromChat(UsersByChat chat)
        {
            return this._repository.getMessagesFromChat(chat);
        }
    }
}
