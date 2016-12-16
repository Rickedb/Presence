using System;

namespace PresenceApp.Core.Entities
{
    public class Chat
    {
        public Chat()
        {

        }

        public int ID { get; set; }

        public string CreatorUsername { get; set; }

        public string Name { get; set; }

        public int Capacity { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? EndDate { get; set; }

        public enum Status
        {
            NOT_ACTIVE,
            ACTIVE
        }
    }
}
