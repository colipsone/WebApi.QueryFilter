using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface IFilterExpressionFactory<TEntity>
    {
        Expression<Func<TEntity, bool>> GetFilterExpression(IFiltering filtering);
    }

    internal class FilterExpressionFactory<TEntity> : IFilterExpressionFactory<TEntity>
    {
        public Expression<Func<TEntity, bool>> GetFilterExpression(IFiltering filtering)
        {
            throw new NotImplementedException();
        }
    }
}