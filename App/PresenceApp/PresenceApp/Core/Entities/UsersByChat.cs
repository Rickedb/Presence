using System;

namespace PresenceApp.Core.Entities
{
    public partial class UsersByChat
    {
        public UsersByChat()
        {
        }

        public int ID { get; set; }

        public string Username { get; set; }

        public int ID_Chat { get; set; }

        public bool Connected { get; set; }

        public DateTime LastInteraction { get; set; }

        public DateTime EntryDate { get; set; }

        public virtual Chat Chat { get; set; }

    }
}
