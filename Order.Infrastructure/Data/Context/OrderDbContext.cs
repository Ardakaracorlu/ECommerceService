﻿using Microsoft.EntityFrameworkCore;
using Order.Infrastructure.Data.Entities;

namespace Order.Infrastructure.Data.Context
{
    public class OrderDbContext : DbContext
    {

        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<OrderInfo>(entity =>
            {
                entity.ToTable("order_info");
                entity.HasQueryFilter(x => !x.DeletedAt.HasValue);
            });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<OrderInfo> OrdersInfo { get; set; }
    }
}
