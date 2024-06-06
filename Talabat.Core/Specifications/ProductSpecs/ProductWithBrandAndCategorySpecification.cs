using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecs
{
	public class ProductWithBrandAndCategorySpecification : BaseSpecifications<Product>
	{
        //This CTOR Will be Use For Creating Object For Get All Product
        public ProductWithBrandAndCategorySpecification(ProductSpecParams productSpec) 
			: base(P => 
					(string.IsNullOrEmpty(productSpec.Search) || P.Name.ToLower().Contains(productSpec.Search))
			        &&
					(!productSpec.BrandId.HasValue || P.ProductBrandId == productSpec.BrandId.Value) 
					&& 
					(!productSpec.TypeId.HasValue || P.ProductTypeId == productSpec.TypeId.Value)
			      )
        {
            Includes.Add(P =>  P.ProductBrand);    
            Includes.Add(P => P.ProductType);

			if(!string.IsNullOrEmpty(productSpec.Sort))
			{
				switch (productSpec.Sort)
				{
					case "priceAsc":
						//OrderBy = P => P.Price;
						AddOrderBy(P => P.Price);
						break;
					case "priceDesc":
						//OrderByDesc = P => P.Price;
						AddOrderByDesc(P => P.Price);
						break;
					default:
						AddOrderBy(P => P.Name);
						break;

				}
			}
			else
			{
				AddOrderBy(P => P.Name);
			}

			ApplyPagination(productSpec.PageSize * (productSpec.PageIndex - 1), productSpec.PageSize);
        }
		public ProductWithBrandAndCategorySpecification(int id) : base(P => P.Id == id)
		{
			Includes.Add(P => P.ProductBrand);
			Includes.Add(P => P.ProductType);
		}

	}
}
