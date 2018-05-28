using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IQueryableFilter.Infrastructure.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public class Filtering : IFiltering
    {
        private readonly IFilterExpressionFactory _filterExpressionFactory;

        private readonly Filter[] _filters;
        private readonly INamedFilterExpressionFactory _namedFilterExpressionFactory;

        public Filtering(IDictionary<string, string[]> queryStringParameters,
            IFilterExpressionFactory filterExpressionFactory,
            INamedFilterExpressionFactory namedFilterExpressionFactory)
        {
            _filterExpressionFactory = filterExpressionFactory;
            _namedFilterExpressionFactory = namedFilterExpressionFactory;
            _filters = ParseFilters(queryStringParameters);
        }

        public Expression<Func<TEntity, bool>> GetNamedFiltersPredicate<TEntity>() where TEntity : class, new()
        {
            return _namedFilterExpressionFactory.GetNamedFilterPredicates<TEntity>(GetNamedFilters())
                .Aggregate((current, predicate) => current.Or(predicate));
        }

        public Expression<Func<TEntity, bool>> GetQueryFiltersPredicate<TEntity>() where TEntity : class, new()
        {
            return _filterExpressionFactory.GetFilterExpression<TEntity>(GetQueryFilters());
        }

        private Filter[] GetNamedFilters()
        {
            return _filters.Where(filter => filter.Operation == FilterOperation.Named).ToArray();
        }

        private Filter[] GetQueryFilters()
        {
            return _filters.Where(filter => filter.Operation != FilterOperation.Named).ToArray();
        }

        private static string GetPropertyNameInCamelCase(IEnumerable<string> values)
        {
            return string.Join(".",
                values
                    .Select(val => string.Join("",
                        val.Split('_').Select(val1 => val1[0].ToString().ToUpper() + val1.Substring(1)))));
        }

        private static string GetSourcePropertyName(IEnumerable<string> values)
        {
            return string.Join(".", values);
        }

        private static (string[] fieldKeys, string operation) ParseFilterItem(string input)
        {
            string[] values = input.Split('[', ']').Where(item => !string.IsNullOrEmpty(item)).ToArray();
            return (fieldKeys: values.Skip(1).Take(values.Length - 2).ToArray(), operation: values.Last());
        }

        private static Filter[] ParseFilters(IDictionary<string, string[]> queryStringParameters)
        {
            return queryStringParameters.Where(param => param.Key.StartsWith("filter["))
                .Select(param =>
                {
                    (string[] fieldKeys, string operation) = ParseFilterItem(param.Key);
                    FilterOperation filterOperation = ParseOperation(operation);
                    string propertyName = GetPropertyNameInCamelCase(fieldKeys);
                    string sourcePropertyName = GetSourcePropertyName(fieldKeys);
                    return param.Value.Select(val => new Filter
                    {
                        Operation = filterOperation,
                        PropertyName = propertyName,
                        PropertyNameSource = sourcePropertyName,
                        Value = val
                    });
                }).SelectMany(res => res).ToArray();
        }

        private static FilterOperation ParseOperation(string stringOperation)
        {
            switch (stringOperation)
            {
                case "eq":
                {
                    return FilterOperation.Equals;
                }
                case "gt":
                {
                    return FilterOperation.GreaterThan;
                }
                case "gte":
                {
                    return FilterOperation.GreaterThanOrEqual;
                }
                case "lt":
                {
                    return FilterOperation.LessThan;
                }
                case "lte":
                {
                    return FilterOperation.LessThanOrEqual;
                }
                case "named":
                {
                    return FilterOperation.Named;
                }

                default:
                    throw new NotSupportedException($"Filter operation {stringOperation} is not supported!");
            }
        }
    }
}