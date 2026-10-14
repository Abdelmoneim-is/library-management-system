using LibraryDomain.Common;
using LibraryDomain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApplication.Specifications
{
    internal abstract class BaseSpecification<TEntity, TKey> : ISpecifications<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        public ICollection<Expression<Func<TEntity, object>>> expressions { get; } = [];

        public Expression<Func<TEntity, bool>> Cerateria { get; private set; }

        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaginated { get; private set; }

        protected void ApplyPagination (int pageSize , int pageIndex)
        {
            IsPaginated = true;
            Take = pageSize;
            Skip = (pageIndex - 1) * pageSize;
        }
        protected void AddOrder (Expression<Func<TEntity , object>> orderBy)
        {
            OrderBy = orderBy;
        }

        protected void AddOrderByDescinding(Expression<Func<TEntity, object>> orderByDescinding)
        {
            OrderByDescending = orderByDescinding;
        }
        protected void AddInclude (Expression<Func<TEntity , object>> include)
        {
            expressions.Add(include);
        }

        protected BaseSpecification(Expression<Func<TEntity , bool>> cerateria)
        {
            Cerateria = cerateria;
        }
    }
}
