﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevExtremeFilterBuilder.Tests
{
    class ProductsContext : DbContext
    {
                
        public DbSet<Product> Products { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseInMemoryDatabase("test");
            optionsBuilder.UseSqlite("Data Source=test.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasKey(c => c.Name);

            modelBuilder.Entity<Product>()
                .HasOne(b => b.Manufacturer);

            modelBuilder.Entity<Product>()
                .Navigation(b => b.Manufacturer)
                .UsePropertyAccessMode(PropertyAccessMode.Property);
        }
    }

}
