using LibraryDomain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryDomain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChanges(CancellationToken ct = default);
        IGenericRepository<TEntity, TKey> GetGenericRepository<TEntity, TKey>() where TEntity : BaseEntity<TKey>;
    }
}
