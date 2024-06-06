using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Specifications;
using Talabat.Repositry.Data;
using Talabat.Repositry.Specifications;

namespace Talabat.Repositry.Repositories
{
	public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
	{
		private readonly StoreDbContext _context;

		public GenericRepository(StoreDbContext context) //ask CLR to inject object
        {
			_context = context;
		}
        public async Task<IReadOnlyList<T>> GetAllAsync()
		{
			if(typeof(T)==typeof(Product))
			{
				return (IReadOnlyList<T>) await _context.Products.Include(P => P.ProductBrand).Include(P => P.ProductType).ToListAsync();
			}
			return await _context.Set<T>().ToListAsync();
		}

		public async Task<IReadOnlyList<T>> GetAllWithSpecAsync(ISpecifications<T> spec)
		{
			return await ApplySpecification(spec).ToListAsync();
		}

		public async Task<T?> GetAsync(int id)
		{
			if (typeof(T) == typeof(Product))
			{
				return await _context.Products.Where(P => P.Id == id).Include(P => P.ProductBrand).Include(P => P.ProductType).FirstOrDefaultAsync() as T;
			}
			return await _context.Set<T>().FindAsync(id);
		}		

		public async Task<T?> GetWithSpecAsync(ISpecifications<T> spec)
		{
			return await ApplySpecification(spec).FirstOrDefaultAsync();

		}

		public async Task<int> GetCountAsync(ISpecifications<T> spec)
		{
			return await ApplySpecification(spec).CountAsync();
		}

		private IQueryable<T> ApplySpecification(ISpecifications<T> spec)
		{
			return SpecificationEvaluator<T>.GetQuery(_context.Set<T>(), spec);
		}

		public async Task AddAsync(T item) => await _context.Set<T>().AddAsync(item);		
		public void Delete(T item) => _context.Set<T>().Remove(item);
		public void Update(T item) =>  _context.Set<T>().Update(item);
	}
}
