using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;

namespace Talabat.Repositry.Data
{
	public static class StoreDbContextSeed
	{
		//Seed Data
		public static async Task SeedAsync(StoreDbContext _context)
		{
			if(_context.ProductBrands.Count() == 0)
			{
				//Brands
				//1.Read Data From Json File
				var brandData = File.ReadAllText("../Talabat.Repositry/Data/DataSeed/brands.json");
				//2.Convert Json String To The Needed Type
				var Brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandData);

				if (Brands?.Count > 0)
				{
					foreach (var brand in Brands)
					{
						_context.Set<ProductBrand>().Add(brand);
					}
					await _context.SaveChangesAsync();
				}
			}
			
			//==============================================================================

			if(_context.ProductTypes.Count() == 0)
			{
				//Category
				//1.Read Data From Json File
				var categoryData = File.ReadAllText("../Talabat.Repositry/Data/DataSeed/categories.json");
				//2.Convert Json String To The Needed Type
				var Categories = JsonSerializer.Deserialize<List<ProductType>>(categoryData);

				if (Categories?.Count > 0)
				{
					foreach (var category in Categories)
					{
						_context.Set<ProductType>().Add(category);
					}
					await _context.SaveChangesAsync();
				}
			}
			
			//===============================================================================

			if(_context.Products.Count() == 0)
			{
				//Product
				//1.Read Data From Json File
				var productData = File.ReadAllText("../Talabat.Repositry/Data/DataSeed/products.json");
				//2.Convert Json String To The Needed Type
				var Products = JsonSerializer.Deserialize<List<Product>>(productData);

				if (Products?.Count > 0)
				{
					foreach (var product in Products)
					{
						_context.Set<Product>().Add(product);
					}
					await _context.SaveChangesAsync();
				}

			}

			//==================================================================================

			if (_context.DeliveryMethods.Count() == 0)
			{
				//Product
				//1.Read Data From Json File
				var deliveryData = File.ReadAllText("../Talabat.Repositry/Data/DataSeed/delivery.json");
				//2.Convert Json String To The Needed Type
				var DeliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryData);

				if (DeliveryMethods?.Count > 0)
				{
					foreach (var deliveryMethod in DeliveryMethods)
					{
						_context.Set<DeliveryMethod>().Add(deliveryMethod);
					}
					await _context.SaveChangesAsync();
				}

			}


		}
	}
}
