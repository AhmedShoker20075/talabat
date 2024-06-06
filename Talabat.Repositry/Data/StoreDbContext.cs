using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Repositry.Data.Configrations;

namespace Talabat.Repositry.Data
{
	public class StoreDbContext : DbContext
	{
        public StoreDbContext(DbContextOptions<StoreDbContext> options):base(options)
        {
            
        }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
            //modelBuilder.ApplyConfiguration(new ProductConfigrations());
            //modelBuilder.ApplyConfiguration(new ProductBrandConfigrations());
            //modelBuilder.ApplyConfiguration(new ProductCategoryConfigrations());

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

			base.OnModelCreating(modelBuilder);
		}

        public DbSet<Product> Products { get; set; }
		public DbSet<ProductBrand> ProductBrands { get; set; }
		public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
        
    }
}
