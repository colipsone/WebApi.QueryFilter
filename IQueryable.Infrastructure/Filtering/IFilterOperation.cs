using System.Collections;
using System.Collections.Generic;
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
        Named,
        Group
    }

    public interface IFilterGroupOperation : IFilterOperation
    {
        IEnumerable<IFilterGroupOperation> Operations { get; set; }
    }
}