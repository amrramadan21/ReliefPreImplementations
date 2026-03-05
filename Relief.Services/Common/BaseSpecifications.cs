using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Services.Common
{
    public abstract class BaseSpecifications<TEntity, TKey>
     : ISpecifications<TEntity, TKey>
     where TEntity : class
    {
        public Expression<Func<TEntity, bool>>? Criteria { get; }

        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; } = [];

        public ICollection<string> IncludeStrings { get; } = new List<string>();

        protected BaseSpecifications()
        {

        }

        protected BaseSpecifications(Expression<Func<TEntity, bool>> criteriaExpression)
        {
            Criteria = criteriaExpression;
        }

        #region Includes

        protected void AddInclude(Expression<Func<TEntity, object>> includeExpression)
        {
            IncludeExpressions.Add(includeExpression);
        }

        protected void AddInclude(string includeString)
        {
            IncludeStrings.Add(includeString);
        }

        #endregion

        #region Sorting

        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

        public Expression<Func<TEntity, object>>? OrderByDescending { get; private set; }

        protected void AddOrderBy(Expression<Func<TEntity, object>> orderByExpression)
        {
            OrderBy = orderByExpression;
        }

        protected void AddOrderByDescending(Expression<Func<TEntity, object>> orderByDescendingExpression)
        {
            OrderByDescending = orderByDescendingExpression;
        }

        #endregion

        #region Pagination

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public bool IsPaginated { get; private set; }

        protected void ApplyPagination(int pageIndex, int pageSize)
        {
            IsPaginated = true;

            Take = pageSize;

            Skip = pageSize * (pageIndex - 1);
        }

        #endregion
    }
}
