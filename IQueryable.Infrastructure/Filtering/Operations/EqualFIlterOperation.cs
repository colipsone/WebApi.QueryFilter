using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal class EqualFilterOperation : RegularFilterOperationBase, IFilterOperation
    {
        public EqualFilterOperation(IFilterValueParserFactory filterValueParserFactory,
            IPropertyInfoResolver propertyInfoResolver)
            : base(filterValueParserFactory, propertyInfoResolver)
        {
        }

        public string OperationShorthand => "eq";

        public FilterOperationType Type => FilterOperationType.Regular;

        public override Expression GetOperationExpression(Expression left, Expression right)
        {
            return Expression.Equal(left, right);
        }
    }
}