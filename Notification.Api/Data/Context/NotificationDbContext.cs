using Microsoft.EntityFrameworkCore;
using Notification.Api.Data.Entities;

namespace Notification.Api.Data.Context
{
    public class NotificationDbContext : DbContext
    {

        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<NotificationInfo>(entity =>
            {
                entity.ToTable("NotificationInfo");
                entity.HasQueryFilter(x => !x.DeletedAt.HasValue);
            });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<NotificationInfo> NotificationInfo { get; set; }
    }
}
