using System;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface IFilterExpressionFactory
    {
        Expression<Func<TEntity, bool>> GetFilterExpression<TEntity>(IFiltering filtering) where TEntity : class, new();
    }
}