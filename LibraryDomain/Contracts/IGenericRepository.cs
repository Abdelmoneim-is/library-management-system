using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Contracts
{
    public interface IGenericRepository<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken ct = default);
        Task<TEntity?> GetByIdAsync(ISpecifications<TEntity , TKey> spec, CancellationToken ct = default);

        Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken ct = default);
        Task<IReadOnlyList<TEntity>> GetAllAsync(ISpecifications<TEntity , TKey> spec ,CancellationToken ct = default);
        Task<int> CountAsync(ISpecifications<TEntity, TKey> spec, CancellationToken ct = default);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

    }
}
