using Sociala.Models;
using Microsoft.EntityFrameworkCore;

namespace Sociala.Data
{
    public class AppData : DbContext
    {
        public AppData(DbContextOptions<AppData> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<Friend> Friend { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<Like> Like { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<Report> Report { get; set; }
        public DbSet<Request> Request { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                   .HasOne(s => s.Roles)
                   .WithMany()
                   .HasForeignKey(s => s.RoleId)
                   .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Post>()
                  .HasOne(s => s.User)
                  .WithMany()
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
             .HasKey(r => new { r.UserId,r.PostId});

            modelBuilder.Entity<Comment>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Comment>()
                .HasOne(s => s.Post)
                .WithMany()
                .HasForeignKey(s => s.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>()
            .HasKey(r => new { r.UserId, r.PostId });

            modelBuilder.Entity<Like>()
               .HasOne(s => s.User)
               .WithMany()
               .HasForeignKey(s => s.UserId)
               .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Like>()
                .HasOne(s => s.Post)
                .WithMany()
                .HasForeignKey(s => s.PostId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Friend>()
                .HasOne(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.RequestingUserId)
                .OnDelete(DeleteBehavior.NoAction);

         

            modelBuilder.Entity<Notification>()
              .HasOne(s => s.User)
              .WithMany()
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Report>()
             .HasOne(s => s.User)
             .WithMany()
             .HasForeignKey(s => s.UserId)
             .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Report>()
                .HasOne(s => s.Post)
                .WithMany()
                .HasForeignKey(s => s.PostId)
                .OnDelete(DeleteBehavior.NoAction);



            modelBuilder.Entity<Request>()
               .HasOne(s => s.User)
               .WithMany()
               .HasForeignKey(s => s.RequestingUserId)
               .OnDelete(DeleteBehavior.NoAction);






        }

    }
}
