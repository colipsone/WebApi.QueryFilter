using System;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public enum FilterOperation
    {
        Equals,
        GreaterThan,
        LessThan,
        GreaterThanOrEqual,
        LessThanOrEqual,
        Named
    }

    public static class FilterOperationExtensions
    {
        public static string ToSqlOperator(this FilterOperation operation)
        {
            switch (operation)
            {
                case FilterOperation.Equals:
                    return "==";
                case FilterOperation.GreaterThan:
                    return ">";
                case FilterOperation.LessThan:
                    return "<";
                case FilterOperation.GreaterThanOrEqual:
                    return ">=";
                case FilterOperation.LessThanOrEqual:
                    return "<=";
                case FilterOperation.Named:
                    throw new NotSupportedException("Named filters should never be involved in dynamic expressions generation process!");
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation), operation, null);
            }
        }
    }
}