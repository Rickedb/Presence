using System.Linq;
using Dapper;
using System.Collections.Generic;
using Presence.Core.Context;
using Presence.Core.Entities;
using Presence.Core.Interfaces.Repository;

namespace Presence.Core.Repository
{
    public class ChatRepository : BaseRepository<Chat>, IChatRepository
    {
        public ChatRepository(PresenceContext context) : base(context)
        {

        }

        public IEnumerable<Chat> getAvailableChats()
        {
            var cn = this._context.Database.Connection;

            var sql = @"SELECT * FROM Chat WHERE EndDate IS NULL";

            return cn.Query<Chat>(sql);
        }
    }
}
