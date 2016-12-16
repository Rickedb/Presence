namespace Presence.Core.Context
{
    using System.Data.Entity;
    using Entities;

    public partial class PresenceContext : DbContext
    {
        public PresenceContext()
            : base("name=PresenceContext")
        {
            this.Configuration.AutoDetectChangesEnabled = false;
        }

        public virtual DbSet<Chat> Chat { get; set; }
        public virtual DbSet<Messages> Messages { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<UsersByChat> UsersByChat { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Chat>()
                .Property(e => e.CreatorUsername)
                .IsUnicode(false);

            modelBuilder.Entity<Chat>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Chat>()
                .HasMany(e => e.UsersByChat)
                .WithRequired(e => e.Chat)
                .HasForeignKey(e => e.ID_Chat)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Messages>()
                .Property(e => e.Message)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.Chat)
                .WithRequired(e => e.Users)
                .HasForeignKey(e => e.CreatorUsername)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Users>()
                .HasMany(e => e.UsersByChat)
                .WithRequired(e => e.Users)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UsersByChat>()
                .Property(e => e.Username)
                .IsUnicode(false);

            modelBuilder.Entity<UsersByChat>()
                .HasMany(e => e.Messages)
                .WithRequired(e => e.UsersByChat)
                .WillCascadeOnDelete(false);
        }
    }
}
