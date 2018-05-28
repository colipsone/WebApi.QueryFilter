using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class GreaterThanOrEqualFilterOperation : IFilterOperation
    {
        public string OperationShorthand => "gte";

        public FilterOperationType Type => FilterOperationType.Regular;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            return Expression.GreaterThanOrEqual(left, right);
        }
    }
}