using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface INamedFilterExpressionFactory
    {
        Expression<Func<TEntity, bool>>[] GetNamedFilterPredicates<TEntity>(Filter[] namedFilters) where TEntity : class, new();
    }
}