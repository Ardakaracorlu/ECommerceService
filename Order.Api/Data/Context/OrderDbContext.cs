using Microsoft.EntityFrameworkCore;
using Order.Api.Data.Entities;

namespace Order.Api.Data.Context
{
    public class OrderDbContext : DbContext
    {

        public OrderDbContext(DbContextOptions<OrderDbContext> options) :base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.ToTable("OrderInfo");
                entity.HasQueryFilter(x => !x.DeletedAt.HasValue);
            });

            base.OnModelCreating(modelBuilder); 
        }


        public DbSet<OrderInfo> OrdersInfo { get; set; }
    }
}
