using Relief.Services.Common;
using Shared.QueryDTOs;
using System.Linq.Expressions;

namespace Relief.Services.Common
{
    public class SmartSpecification<TEntity, TKey>
        : BaseSpecifications<TEntity, TKey>
        where TEntity : class
    {
        public SmartSpecification(
            BaseQueryParams query,
            Expression<Func<TEntity, bool>>? criteria = null)
            : base(criteria!)
        {
            // pagination
            ApplyPagination(query.PageIndex, query.PageSize);
        }
    }
}