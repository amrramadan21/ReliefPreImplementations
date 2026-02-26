using Relief.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Relief.Domain.Contracts
{
    public interface ISpecifications<TEntity, TKey> where TEntity : class
    {
        public ICollection<Expression<Func<TEntity, object>>> IncludeExpressions { get; }
        ICollection<string> IncludeStrings { get; }
        public Expression<Func<TEntity, bool>>? Criteria { get; }
        public Expression<Func<TEntity, object>> OrderBy { get; }
        public Expression<Func<TEntity, object>> OrderByDescending { get; }
        public int Skip { get;}
        public int Take { get;}
        public bool IsPaginated { get;}
    }
}
