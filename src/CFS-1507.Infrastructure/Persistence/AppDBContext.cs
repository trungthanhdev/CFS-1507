using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CFS_1507.Domain.Entities;
using CFS_1507.Infrastructure.SeedData;
using Microsoft.EntityFrameworkCore;

namespace CFS_1507.Infrastructure.Persistence
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public AppDbContext() { }
        public DbSet<LanguageEntity> LanguageEntities { get; set; }
        public DbSet<ProductEntity> ProductEntities { get; set; }
        public DbSet<UserEntity> UserEntities { get; set; }
        public DbSet<BlackListEntity> BlackListEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new LanguageEntityConfiguration());
        }
    }
}