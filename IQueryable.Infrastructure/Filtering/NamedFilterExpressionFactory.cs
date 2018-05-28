using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Autofac;
using Autofac.Features.Metadata;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class NamedFilterExpressionFactory : INamedFilterExpressionFactory
    {
        private readonly ILifetimeScope _lifetimeScope;

        public NamedFilterExpressionFactory(ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
        }

        public Expression<Func<TEntity, bool>>[] GetNamedFilterPredicates<TEntity>(Filter[] namedFilters)
            where TEntity : class, new()
        {
            if (namedFilters.Length == 0)
            {
                return new[] {(Expression<Func<TEntity, bool>>) (entity => true)};
            }
            return namedFilters.Select(filter =>
                {
                    Meta<INamedFilter> namedFilter = _lifetimeScope.Resolve<IEnumerable<Meta<INamedFilter>>>()
                        .FirstOrDefault(cmd => cmd.Metadata["FilterName"].Equals(filter.Value));

                    if (namedFilter == null)
                        throw new ApplicationException(
                            $"There is no named filter: '{filter.Value}' registered in a system.");

                    if (!(namedFilter.Value is INamedFilter<TEntity> filterValue))
                        throw new ApplicationException(
                            $"Named filter: '{filter.Value}' can't be used here. It is designed for another query.");

                    return filterValue.GetFilterPredicate();
                }
            ).ToArray();
        }
    }
}