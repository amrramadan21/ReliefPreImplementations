using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Contracts
{
    public interface IGenaricRepository<TEntity, TKey> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TKey id);
        Task<IEnumerable<TEntity>> GetAllAsync(ISpecifications<TEntity, TKey> specifications);
        Task<TEntity?> GetByIdAsync(ISpecifications<TEntity, TKey> specifications);

        //Task<List<JobOffer>> GetPagedAsync(int pageNumber, int pageSize);
        Task AddAsync(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
