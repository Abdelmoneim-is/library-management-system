using LibraryDomain.Common;
using LibraryDomain.Contracts;
using LibraryInfrastructure.Data;
using LibraryInfrastructure.Specifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    internal class GenericRepository<TEntity, TKey>(StoreDbContext dbContext) : IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        private readonly StoreDbContext _dbContext = dbContext;

        public void Add(TEntity entity)
        {
            _dbContext.Add(entity);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(ISpecifications<TEntity, TKey> spec, CancellationToken ct = default)
        {
            return SpecificationEvaluator.CreateQuery(_dbContext.Set<TEntity>(), spec).CountAsync(ct);

        }

        public void Delete(TEntity entity)
        {
            _dbContext.Remove(entity);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default)
        {
           return await _dbContext.Set<TEntity>().ToListAsync(ct);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> spec, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.CreateQuery(_dbContext.Set<TEntity>(), spec);
            return await query.ToListAsync(ct);
        }

        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default)
        {
            return await _dbContext.Set<TEntity>().FindAsync(id, ct);
        }

        public async Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> spec, CancellationToken ct = default)
        {
            var query = SpecificationEvaluator.CreateQuery(_dbContext.Set<TEntity>(), spec);
            return await query.FirstOrDefaultAsync(ct);
        }

        public void Update(TEntity entity)
        {
             _dbContext.Update(entity);
        }
    }
}
