using Relief.Services.Common;
using System.Linq.Expressions;

public class PaginationSpecification<TEntity, TKey>
       : BaseSpecifications<TEntity, TKey>
       where TEntity : class
{
    public PaginationSpecification(
        int pageIndex,
        int pageSize)
        : base()
    {
        ApplyPagination(pageIndex, pageSize);
    }

    public PaginationSpecification(
        Expression<Func<TEntity, bool>> criteria)
        : base(criteria)
    {
    }

    public PaginationSpecification(
        Expression<Func<TEntity, bool>> criteria,
        int pageIndex,
        int pageSize)
        : base(criteria)
    {
        ApplyPagination(pageIndex, pageSize);
    }

    public PaginationSpecification(
        Expression<Func<TEntity, bool>> criteria,
        Expression<Func<TEntity, object>> orderBy,
        int pageIndex,
        int pageSize)
        : base(criteria)
    {
        AddOrderBy(orderBy);

        ApplyPagination(pageIndex, pageSize);
    }

    public PaginationSpecification(
        Expression<Func<TEntity, bool>> criteria,
        Expression<Func<TEntity, object>> orderBy,
        bool desc,
        int pageIndex,
        int pageSize)
        : base(criteria)
    {
        if (desc)
            AddOrderByDescending(orderBy);
        else
            AddOrderBy(orderBy);

        ApplyPagination(pageIndex, pageSize);
    }
}