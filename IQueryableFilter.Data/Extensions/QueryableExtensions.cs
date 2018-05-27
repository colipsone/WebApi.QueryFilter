using System.Linq;
using IQueryableFilter.Infrastructure.Filtering;

namespace IQueryableFilter.Data.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> FilterByNamed<TEntity>(this IQueryable<TEntity> query, IFiltering filtering) where TEntity : class, new()
        {
            return query.Where(filtering.GetNamedFiltersPredicate<TEntity>());
        }

        public static IQueryable<TEntity> FilterBy<TEntity>(this IQueryable<TEntity> query, IFiltering filtering) where TEntity : class, new()
        {
            return query.Where(filtering.GetQueryFiltersPredicate<TEntity>());
        }
    }
}