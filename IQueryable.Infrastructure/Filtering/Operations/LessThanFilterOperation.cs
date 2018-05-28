using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class LessThanFilterOperation : IFilterOperation
    {
        public string OperationShorthand => "lt";

        public FilterOperationType Type => FilterOperationType.Regular;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            return Expression.LessThan(left, right);
        }
    }
}