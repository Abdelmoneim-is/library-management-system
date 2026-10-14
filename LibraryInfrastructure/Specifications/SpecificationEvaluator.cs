using LibraryDomain.Common;
using LibraryDomain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Specifications
{
    internal static class SpecificationEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity , TKey> (IQueryable<TEntity> inputquery , ISpecifications <TEntity , TKey> spec) where TEntity : BaseEntity<TKey>
        {
            var query = inputquery;
            if (spec.Cerateria != null)
            {
                query = query.Where(spec.Cerateria);
            }
            if(spec.expressions.Any())
            {
                query = spec.expressions.Aggregate(query, (current, next) => current.Include(next));
            }
            if (spec.OrderBy != null)
            {
                query = query.OrderBy(spec.OrderBy);
            }
            else if (spec.OrderByDescending != null)
            {
                query = query.OrderByDescending(spec.OrderByDescending);
            }

            if (spec.IsPaginated)
            {
                query = query.Skip(spec.Skip).Take(spec.Take);
            }
                return query;
        }
    }
}
