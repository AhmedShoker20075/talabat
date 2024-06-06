using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Interfaces;
using Talabat.Core.Services.Interfaces;
using Talabat.Repositry.Data;
using Talabat.Repositry.Repositories;

namespace Talabat.Repositry
{
	public class UnitOfWork : IUnitOfWork
	{
		private readonly StoreDbContext _context;
		public Hashtable _Repositories;

        public UnitOfWork(StoreDbContext context)
        {
			_context = context;
			_Repositories = new Hashtable();	
		}
        public Task<int> CompleteAsync() => _context.SaveChangesAsync();

		public ValueTask DisposeAsync() => _context.DisposeAsync();

		public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
		{
			var type = typeof(TEntity).Name;
			
			if(!_Repositories.ContainsKey(type))
			{
				var repository = new GenericRepository<TEntity>(_context);
				_Repositories.Add(type, repository);
			}
			
			return _Repositories[type] as IGenericRepository<TEntity>;
		}
	}
}
