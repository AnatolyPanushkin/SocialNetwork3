using Microsoft.EntityFrameworkCore;
using SocialNetwork.Domain.Aggregates;
using SocialNetwork.Domain.Aggregates.UserAggregate;

namespace SocialNetwork.Infrastructure.Data
{
    public class SocialNetworkContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Publication> Publications { get; set; }

        public SocialNetworkContext(DbContextOptions<SocialNetworkContext> options) : base(options)
        {
        }

        public SocialNetworkContext()
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasKey(k => k.Id);

            modelBuilder.Entity<User>().ToTable(nameof(User));

            modelBuilder.Entity<User>().Property(r => r.Id).ValueGeneratedNever();
            modelBuilder.Entity<Publication>().Property(r => r.Id).ValueGeneratedNever();

            modelBuilder.Entity<User>().OwnsOne(x => x.UserName,
                a =>
                {
                    a.Property(p => p.FirstName)
                        .HasColumnName(nameof(User.UserName.FirstName))
                        .HasMaxLength(50)
                        .IsRequired();
                });

            modelBuilder.Entity<User>().OwnsOne(x => x.UserName,
                a =>
                {
                    a.Property(p => p.LastName)
                        .HasColumnName(nameof(User.UserName.LastName))
                        .HasMaxLength(50)
                        .IsRequired();
                });

            modelBuilder.Entity<User>().OwnsOne(x => x.Birthday,
                a =>
                {
                    a.Property(p => p.Date)
                        .HasColumnName(nameof(User.Birthday))
                        .IsRequired();
                });

            modelBuilder.Entity<User>()
                .HasMany(user => user.Publications)
                .WithOne(publication => publication.User);

            modelBuilder.Entity<UsersFriends>(entity =>
            {
                entity.HasKey(source => new { source.UserId, source.UsersFriendId });
            });

        }
    }
}
