using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class GreaterThanFilterOperation : IFilterOperation
    {
        public string OperationShorthand => "gt";

        public FilterOperationType Type => FilterOperationType.Regular;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            return Expression.GreaterThan(left, right);
        }
    }
}