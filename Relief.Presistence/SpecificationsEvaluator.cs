using Relief.Domain.Contracts;
using Relief.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_CommerceLyaer.Presistence
{
    internal static class SpecificationsEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity, TKey>(IQueryable<TEntity> entryPoint, ISpecifications<TEntity, TKey> specifications) where TEntity : class
        {
            var query = entryPoint;
            if (specifications is not null)
            {
                if(specifications.Criteria is not null)
                {
                    query = query.Where(specifications.Criteria);
                }
                if (specifications.IncludeExpressions is not null && specifications.IncludeExpressions.Any())
                {
                    // instead of foreach we can use Aggregate
                    query = specifications.IncludeExpressions
                        .Aggregate(query, (current, includeExpression) => current.Include(includeExpression));
                }
                if (specifications.IncludeStrings is not null && specifications.IncludeStrings.Any())
                {
                    query = specifications.IncludeStrings
                        .Aggregate(query, (current, include) => current.Include(include));
                }
                if (specifications.OrderBy is not null)
                {
                    query = query.OrderBy(specifications.OrderBy);
                }
                if(specifications.OrderByDescending is not null)
                {
                    query = query.OrderByDescending(specifications.OrderByDescending);
                }
                if(specifications.IsPaginated)
                {
                    query = query.Skip(specifications.Skip).Take(specifications.Take);
                }
            }
            return query;
        }
    }
}
