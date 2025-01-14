﻿using Microsoft.EntityFrameworkCore;
using Stock.Infrastructure.Data.Entities;

namespace Stock.Infrastructure.Data.Context
{
    public class StockDbContext : DbContext
    {

        public StockDbContext(DbContextOptions<StockDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<StockInfo>(entity =>
            {
                entity.ToTable("stock_info");
                entity.HasQueryFilter(x => !x.DeletedAt.HasValue);
            });

            base.OnModelCreating(modelBuilder);
        }


        public DbSet<StockInfo> StocksInfo { get; set; }
    }
}
