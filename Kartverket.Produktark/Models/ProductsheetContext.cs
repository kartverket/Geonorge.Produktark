using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Kartverket.Produktark.Models
{
    public class ProductSheetContext : System.Data.Entity.DbContext
    {
        public ProductSheetContext():base("DefaultConnection"){
        }

        public DbSet<ProductSheet> ProductSheet { get; set; }
        public DbSet<Logo> Logo { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Logo>().ToTable("Logos");
        }
    }
}