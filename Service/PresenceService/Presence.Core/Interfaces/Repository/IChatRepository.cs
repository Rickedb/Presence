﻿using Presence.Core.Entities;
using System.Collections.Generic;

namespace Presence.Core.Interfaces.Repository
{
    public interface IChatRepository : IBaseRepository<Chat>
    {
        IEnumerable<Chat> getAvailableChats();
    }
}
