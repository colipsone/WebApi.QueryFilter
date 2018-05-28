using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilterOperation
    {
        string OperationShorthand { get; }

        FilterOperationType Type { get; }

        Expression GetOperationExpression(Expression left, Expression right);
    }

    public enum FilterOperationType
    {
        Regular,
        Named
    }
}