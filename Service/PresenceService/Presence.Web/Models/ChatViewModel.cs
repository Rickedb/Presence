using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Presence.Web.Models
{
    public class ChatViewModel
    {
        public LoginViewModel User { get; set; }

        public int ID { get; set; }

        [DisplayName("Criador")]
        public string CreatorUsername { get; set; }

        [DisplayName("Nome")]
        public string Name { get; set; }

        [DisplayName("Capacidade")]
        public int Capacity { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime? EndDate { get; set; }

        public Status getStatus()
        {
            if (this.EndDate == null)
                return Status.ACTIVE;

            return Status.NOT_ACTIVE;
        }

        public enum Status
        {
            NOT_ACTIVE,
            ACTIVE
        }
    }
}