using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Specifications;

namespace Talabat.Repositry.Specifications
{
	public class SpecificationEvaluator<TEntity> where TEntity : BaseEntity
	{
		public static IQueryable<TEntity> GetQuery(IQueryable<TEntity> inputQuery, ISpecifications<TEntity> spec)
		{
			var query = inputQuery; //_context.Set<Product>()

			if (spec.Criteria is not null)
				query = query.Where(spec.Criteria);
				//_context.Set<Product>().Where(P => P.Id == 10);
			

			if (spec.OrderBy is not null)
				query = query.OrderBy(spec.OrderBy);
			

			if (spec.OrderByDesc is not null)
				query = query.OrderByDescending(spec.OrderByDesc);

			if (spec.IsPaginationEnabled)
				query = query.Skip(spec.Skip).Take(spec.Take);

			//Includes 
			//1. P => P.Brands
			//1. P => P.BCategories

			//ahmed , ali , omar , mahmoud

			query = spec.Includes.Aggregate(query, (currentQuery, includeExpression) => currentQuery.Include(includeExpression));

			return query;
		}
	}
}

//_context.Products.Include(P => P.Brand).Include(P => P.Category).ToListAsync();
//_context.Products.Where(P => P.Id == id).Include(P => P.Brand).Include(P => P.Category).FirstOrDefaultAsync() as T;
