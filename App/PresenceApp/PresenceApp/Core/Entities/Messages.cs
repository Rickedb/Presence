using System;

namespace PresenceApp.Core.Entities
{
    public partial class Messages
    {
        public int ID { get; set; }

        public int UsersByChatID { get; set; }

        public string Message { get; set; }

        public Types Type { get; set; }

        public DateTime InsertedDate { get; set; }

        public enum Types
        {
            STRING
        }
    }
}
