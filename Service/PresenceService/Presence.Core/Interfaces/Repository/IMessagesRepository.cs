using Presence.Core.Entities;
using System.Collections.Generic;

namespace Presence.Core.Interfaces.Repository
{
    public interface IMessagesRepository : IBaseRepository<Messages>
    {
        IEnumerable<Messages> getLastFifty(Chat chat);
        IEnumerable<Messages> getMessagesFromChat(UsersByChat chat);
    }
}
