using System;
using System.Collections.Generic;
using Presence.Core.Context;
using Presence.Core.Entities;
using Presence.Core.Interfaces.Repository;

namespace Presence.Core.Repository
{
    public class UsersRepository : BaseRepository<Users>, IUsersRepository
    {
        public UsersRepository(PresenceContext context) : base(context)
        {

        }

        public IEnumerable<Users> getAdministrators()
        {
            throw new NotImplementedException();
        }
    }
}
