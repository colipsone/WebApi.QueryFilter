using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class OrFilterGroupOperation : IFilterGroupOperation
    {
        public string OperationShorthand => "or";

        public FilterOperationType Type => FilterOperationType.Group;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFilterGroupOperation> Operations { get; set; }
    }
}