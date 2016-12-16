using System;

namespace PresenceApp.Core.Entities
{
    public partial class Users
    {
        public Users()
        {

        }

        public string Username { get; set; }

        public bool Admin { get; set; }

        public string Password { get; set; }

        public DateTime CreationDate { get; set; }

    }
}
