using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class EqualFilterOperation: IFilterOperation
    {
        public string OperationShorthand => "eq";

        public FilterOperationType Type => FilterOperationType.Regular;

        public Expression GetOperationExpression(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }
    }
}