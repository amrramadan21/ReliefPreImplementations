using Relief.Domain.Contracts;
using Shared.QueryDTOs;

namespace Relief.Services.Common
{
    public static class PaginationHelper
    {
        public static async Task<Pagination<TEntity>> CreatePagedResult<TEntity, TKey>(
            IGenaricRepository<TEntity, TKey> repository,
            PaginationSpecification<TEntity, TKey> spec,
            PaginationSpecification<TEntity, TKey> countSpec,
            int pageIndex,
            int pageSize)
            where TEntity : class
        {
            var data = await repository.GetAllAsync(spec);

            var totalCount = await repository.CountAsync(countSpec);

            return new Pagination<TEntity>(
                pageIndex,
                pageSize,
                totalCount,
                data.ToList());
        }
    }
}