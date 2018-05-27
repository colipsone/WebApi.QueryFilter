using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFiltering
    {
        string GetFilterSetUniqueName();

        Filter[] GetNamedFilters();

        Expression<Func<TEntity, bool>> GetNamedFiltersPredicate<TEntity>() where TEntity : class, new();

        Filter[] GetQueryFilters();

        Expression<Func<TEntity, bool>> GetQueryFiltersPredicate<TEntity>() where TEntity : class, new();
    }
}