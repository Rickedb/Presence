using System.Collections.Generic;
using Presence.Core.Entities;

namespace Presence.Core.Interfaces.Services
{
    public interface IMessagesServices : IBaseServices<Messages>
    {
        IEnumerable<Messages> getLastFifty(Chat chat);
        IEnumerable<Messages> getMessagesFromChat(UsersByChat chat);
    }
}
