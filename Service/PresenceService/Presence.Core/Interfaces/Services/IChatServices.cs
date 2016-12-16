using Presence.Core.Entities;
using System.Collections.Generic;

namespace Presence.Core.Interfaces.Services
{
    public interface IChatServices : IBaseServices<Chat>
    {
        IEnumerable<Chat> getAvailableChats();
    }
}
