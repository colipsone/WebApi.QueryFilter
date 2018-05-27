using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilterExpressionBuilder
    {
        Expression<Func<TEntity, bool>> GetNamedFiltersExpressionPredicate<TEntity>(IFiltering filtering) where TEntity : class, new();

        Expression<Func<TEntity, bool>> GetQueryFiltersExpressionPredicate<TEntity>(IFiltering filtering) where TEntity : class, new();
    }
}