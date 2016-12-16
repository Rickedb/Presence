namespace Presence.Core.Entities
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UsersByChat")]
    public partial class UsersByChat
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsersByChat()
        {
            Messages = new HashSet<Messages>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required]
        [StringLength(20)]
        public string Username { get; set; }

        public int ID_Chat { get; set; }

        public bool Connected { get; set; }

        public DateTime LastInteraction { get; set; }

        public DateTime EntryDate { get; set; }

        public virtual Chat Chat { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Messages> Messages { get; set; }

        public virtual Users Users { get; set; }
    }
}
