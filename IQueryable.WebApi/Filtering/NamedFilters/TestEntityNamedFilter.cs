using System;
using System.Linq.Expressions;
using IQueryableFilter.Infrastructure.Filtering;
using IQueryableFilter.WebApi.Dtos;

namespace IQueryableFilter.WebApi.Filtering
{
    /// <summary>
    ///     The named filter to test entities
    /// </summary>
    public class TestEntityNamedFilter : INamedFilter<TestEntity>
    {
        /// <summary>
        ///     Gets the filter predicate.
        /// </summary>
        /// <returns></returns>
        public Expression<Func<TestEntity, bool>> GetFilterPredicate()
        {
            return ent => ent.UniqueName == "test";
        }
    }
}