using System.Collections.Generic;
using System.Linq;
using PresenceApp.Core.Entities;

namespace PresenceApp
{
    public class ChatCardView
    {
        IEnumerable<Chat> AvailableChats { get; set; }

        public ChatCardView(IEnumerable<Chat> availableChats)
        {
            this.AvailableChats = availableChats;
        }

        public int totalAvailableChats() { return this.AvailableChats.Count(); }

        public Chat this[int i]
        {
            get { return this.AvailableChats.ElementAt(i); }
        }

    }

}