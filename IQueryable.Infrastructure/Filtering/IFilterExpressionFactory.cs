using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilterExpressionFactory
    {
        Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(Filter[] queryFilters) where TEntity : class, new();
    }
}