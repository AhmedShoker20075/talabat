using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
	public class BaseSpecifications<T> : ISpecifications<T> where T : BaseEntity
	{
        public Expression<Func<T, bool>> Criteria { get; set; } = null;
		public List<Expression<Func<T, object>>> Includes { get; set; } = new List<Expression<Func<T, object>>>();
        public Expression<Func<T, object>> OrderBy { get; set; } = null;
        public Expression<Func<T, object>> OrderByDesc { get; set; } = null;
        
		public bool IsPaginationEnabled { get; set; }
		public int Skip { get; set; }
		public int Take { get; set; }

		public BaseSpecifications()
        {
			//Includes = new List<Expression<Func<T, object>>>();
			//Criteria = null;

            //Includes.Add(P => P.Brands);
            //Includes.Add(P => P.Categories);

        }
        public BaseSpecifications(Expression<Func<T, bool>> Citeriaexpression)
        {
            Criteria = Citeriaexpression; //P => P.Id == 10
        }

        public void AddOrderBy(Expression<Func<T, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

		public void AddOrderByDesc(Expression<Func<T, object>> orderByDescExpression)
		{
            OrderByDesc = orderByDescExpression;
		}

        public void ApplyPagination(int skip,int take)
        {
            Skip = skip;
            Take = take;
            IsPaginationEnabled = true;
        }
	}
	
}
