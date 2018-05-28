using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class LessThanOrEqualFilterOperation : IFilterOperation
    {
        public string OperationShorthand => "lte";

        public FilterOperationType Type => FilterOperationType.Regular;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            return Expression.LessThanOrEqual(left, right);
        }
    }
}