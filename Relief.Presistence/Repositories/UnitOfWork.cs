using Relief.Domain.Contracts;
using Relief.Presistence.Data.DbContexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Presistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Dictionary<Type, object> _repositories = [];
        private readonly ReliefAppDbContext _dbContext;

        public UnitOfWork(ReliefAppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IGenaricRepository<TEntity, TKey> GetRepository<TEntity, TKey>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (_repositories.TryGetValue(entityType, out object? repository))
                return (IGenaricRepository<TEntity, TKey>)repository;
            var newRepository = new GenericRepository<TEntity, TKey>(_dbContext);
            _repositories[entityType] = newRepository;
            return newRepository;
        }

        public async Task<int> SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
