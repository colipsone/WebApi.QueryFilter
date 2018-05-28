using System;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class NamedlFilterOperation : IFilterOperation
    {
        public string OperationShorthand => "named";

        public FilterOperationType Type => FilterOperationType.Named;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            throw new NotSupportedException("Named operations is not supported in this context!");
        }
    }
}