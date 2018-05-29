using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFiltering
    {
        Expression<Func<TEntity, bool>> GetNamedFiltersPredicate<TEntity>() where TEntity : class, new();

        Expression<Func<TEntity, bool>> GetQueryFiltersPredicate<TEntity>() where TEntity : class, new();

        void Initialize(IDictionary<string, string[]> queryStringParameters);
    }
}