using LibraryDomain.Common;
using LibraryDomain.Contracts;
using LibraryInfrastructure.Data;
using LibraryDomain.Exceptions; 
using Microsoft.EntityFrameworkCore; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryInfrastructure.Repositories
{
    public class UnitOfWork(StoreDbContext dbContext) : IUnitOfWork
    {
        private readonly Dictionary<string, object> repositories = [];

        public IGenericRepository<TEntity, TKey> GetGenericRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>
        {
            var typeName = typeof(TEntity).Name;
            if (repositories.TryGetValue(typeName, out object? value))
            {
                return (IGenericRepository<TEntity, TKey>)value;
            }
            else
            {
                var repo = new GenericRepository<TEntity, TKey>(dbContext);
                repositories[typeName] = repo;
                return repo;
            }
        }

        public async Task<int> SaveChanges(CancellationToken ct = default)
        {
            try
            {
                return await dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new ConcurrencyException("A concurrency error occurred while saving changes.", ex);
            }
        }
    }
}
