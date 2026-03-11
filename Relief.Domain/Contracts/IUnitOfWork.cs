using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Contracts
{
    public interface IUnitOfWork
    {
        Task<int> SaveChangesAsync();

        IGenaricRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class;

        Task RebuildDatabaseAsync();
    }
}
