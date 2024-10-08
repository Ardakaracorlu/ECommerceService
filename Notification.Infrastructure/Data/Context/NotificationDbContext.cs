using Microsoft.EntityFrameworkCore;
using Notification.Infrastructure.Data.Entities;

namespace Notification.Infrastructure.Data.Context
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
                entity.ToTable("notification_info");
                entity.HasQueryFilter(x => !x.DeletedAt.HasValue);
            });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<NotificationInfo> NotificationInfo { get; set; }
    }
}
