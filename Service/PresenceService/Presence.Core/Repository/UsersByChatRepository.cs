using System.Linq;
using Dapper;
using System.Collections.Generic;
using Presence.Core.Context;
using Presence.Core.Entities;
using Presence.Core.Interfaces.Repository;
using System;

namespace Presence.Core.Repository
{
    public class UsersByChatRepository : BaseRepository<UsersByChat>, IUsersByChatRepository
    {
        public UsersByChatRepository(PresenceContext context) : base(context)
        {
           
        }

        public UsersByChat getUserByChat(string username, int chatId)
        {
            var cn = this._context.Database.Connection;

            var sql = @"SELECT ID,
                               Username,
                               ID_Chat,
                               Connected,
                               LastInteraction,
                               EntryDate
                         FROM UsersByChat
                        WHERE Username = '" + username + @"' 
                         AND ID_Chat = " + chatId;

            return cn.Query<UsersByChat>(sql).FirstOrDefault();
        }

        public IEnumerable<UsersByChat> getUserEnteredChats(Users user)
        {
            var cn = this._context.Database.Connection;

            var sql = @"SELECT ID,
                               Username,
                               ID_Chat,
                               Connected,
                               LastInteraction,
                               EntryDate
                         FROM UsersByChat
                        WHERE Username = '" + user.Username + "'";

            return cn.Query<UsersByChat>(sql); 
        }

        public IEnumerable<UsersByChat> getUsersInChat(Chat chat)
        {
            var cn = this._context.Database.Connection;

            var sql = @"SELECT ID,
                               Username,
                               ID_Chat,
                               Connected,
                               LastInteraction,
                               EntryDate
                         FROM UsersByChat
                        WHERE ID_Chat = " + chat.ID +
                        " AND LastInteraction > '" + DateTime.Now.AddMinutes(-15) + @"'
                          AND Connected = 1";
             
            return cn.Query<UsersByChat>(sql);
        }
    }
}
