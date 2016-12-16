using Presence.Core.Entities;
using System.Collections.Generic;

namespace Presence.Core.Interfaces.Services
{
    public interface IUsersByChatServices : IBaseServices<UsersByChat>
    {
        UsersByChat getUserByChat(string username, int chatId);
        IEnumerable<UsersByChat> getUsersInChat(Chat chat);
        IEnumerable<UsersByChat> getUserEnteredChats(Users user);
    }
}
