namespace Presence.Core.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Messages
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        public int UsersByChatID { get; set; }

        [Required]
        [StringLength(2000)]
        public string Message { get; set; }

        public Types Type { get; set; }

        public DateTime InsertedDate { get; set; }

        public virtual UsersByChat UsersByChat { get; set; }

        public enum Types
        {
            STRING
        }
    }
}
