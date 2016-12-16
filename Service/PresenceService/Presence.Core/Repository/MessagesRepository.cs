using Presence.Core.Context;
using Presence.Core.Entities;
using Presence.Core.Interfaces.Repository;
using System.Collections.Generic;
using Dapper;

namespace Presence.Core.Repository
{
    public class MessagesRepository : BaseRepository<Messages>, IMessagesRepository
    {
        public MessagesRepository(PresenceContext context) : base(context)
        {

        }

        public IEnumerable<Messages> getLastFifty(Chat chat)
        {
            var cn = this._context.Database.Connection;

            var sql = @"SELECT TOP 10 MSG.ID,
                                      MSG.UsersByChatID,
                                      MSG.Message,
                                      MSG.Type,
                                      MSG.InsertedDate,
                                      UBC.Username
                         FROM Messages MSG
                   INNER JOIN UsersByChat UBC ON UBC.ID = MSG.UsersByChatID
                        WHERE UBC.ID_Chat = " + chat.ID + " ORDER BY MSG.ID DESC";

            return cn.Query<Messages, UsersByChat, Messages>(sql, (msg, ubc) => 
            {
                msg.UsersByChat = ubc;
                return msg;
            },  splitOn: "ID, Username");
        }

        public IEnumerable<Messages> getMessagesFromChat(UsersByChat chat)
        {
            var cn = this._context.Database.Connection;

            var sql = @"SELECT ID,
                               UsersByChatID,
                               Message,
                               Type,
                               InsertedDate
                         FROM Messages
                        WHERE UsersByChatID = " + chat.ID;

            return cn.Query<Messages>(sql);
        }
    }
}
