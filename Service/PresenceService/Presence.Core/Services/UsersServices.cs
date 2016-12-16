using Presence.Core.Entities;
using Presence.Core.Interfaces.Repository;
using Presence.Core.Interfaces.Services;

namespace Presence.Core.Services
{
    public class UsersServices : BaseServices<Users>, IUsersServices
    {
        public UsersServices(IUsersRepository repository) : base(repository)
        {

        }
    }
}
