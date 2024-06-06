using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecs
{
	public class ProductWithFilterationForCountSpecifications:BaseSpecifications<Product>
	{
        public ProductWithFilterationForCountSpecifications(ProductSpecParams productSpec) : base(P =>
					(!productSpec.BrandId.HasValue || P.ProductBrandId == productSpec.BrandId.Value)
					&&
					(!productSpec.TypeId.HasValue || P.ProductTypeId == productSpec.TypeId.Value)
				  )
		{
            
        }
    }
}
