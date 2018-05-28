using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilterOperationFactory
    {
        IFilterOperation GetByShorthandName(string name);
    }

    internal class FilterOperationFactory : IFilterOperationFactory
    {
        private readonly Dictionary<string, IFilterOperation> _filterOperations;

        public FilterOperationFactory(IComponentContext lifetimeScope)
        {
            _filterOperations = lifetimeScope.Resolve<IEnumerable<IFilterOperation>>()
                .ToDictionary(op => op.OperationShorthand);
        }

        public IFilterOperation GetByShorthandName(string name)
        {
            return !_filterOperations.ContainsKey(name) ? null : _filterOperations[name];
        }
    }
}