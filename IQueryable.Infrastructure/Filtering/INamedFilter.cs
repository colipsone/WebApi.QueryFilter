using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface INamedFilter
    {
    }

    public interface INamedFilter<TEntity> : INamedFilter where TEntity : class, new()
    {
        Expression<Func<TEntity, bool>> GetFilterPredicate();
    }
}