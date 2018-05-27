using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public class Filtering : IFiltering
    {
        private readonly IFilterExpressionBuilder _expressionBuilder;

        private readonly Filter[] _filters;

        public Filtering(IDictionary<string, string[]> queryStringParameters,
            IFilterExpressionBuilder expressionBuilder)
        {
            _expressionBuilder = expressionBuilder ?? throw new ArgumentNullException(nameof(expressionBuilder));
            _filters = ParseFilters(queryStringParameters);
        }

        public string GetFilterSetUniqueName()
        {
            return string.Join("|",
                _filters.OrderBy(filter => filter.PropertyName)
                    .Select(filter => filter.PropertyName));
        }

        public Filter[] GetNamedFilters()
        {
            return _filters.Where(filter => filter.Operation == FilterOperation.Named).ToArray();
        }

        public Expression<Func<TEntity, bool>> GetNamedFiltersPredicate<TEntity>() where TEntity : class, new()
        {
            return _expressionBuilder.GetNamedFiltersExpressionPredicate<TEntity>(this);
        }

        public Filter[] GetQueryFilters()
        {
            return _filters.Where(filter => filter.Operation != FilterOperation.Named).ToArray();
        }

        public Expression<Func<TEntity, bool>> GetQueryFiltersPredicate<TEntity>() where TEntity : class, new()
        {
            return _expressionBuilder.GetQueryFiltersExpressionPredicate<TEntity>(this);
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
                    (string[] fieldKeys, var operation) = ParseFilterItem(param.Key);
                    FilterOperation filterOperation = ParseOperation(operation);
                    var propertyName = GetPropertyNameInCamelCase(fieldKeys);
                    var sourcePropertyName = GetSourcePropertyName(fieldKeys);
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