using Presence.Core.Entities;
using Presence.Core.Interfaces.Services;
using System.Collections.Generic;
using Presence.Core.Interfaces.Repository;

namespace Presence.Core.Services
{
    public class ChatServices : BaseServices<Chat>, IChatServices
    {
        private readonly IChatRepository _repository;

        public ChatServices(IChatRepository repository) : base(repository)
        {
            this._repository = repository;
        }

        public IEnumerable<Chat> getAvailableChats()
        {
            return this._repository.getAvailableChats();
        }
    }
}
