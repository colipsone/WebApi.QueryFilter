using System;
using System.Collections.Generic;
using System.Linq;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public class QueryStringFilterItemsParser : IFilterItemsParser
    {
        private readonly IFilterOperationFactory _filterOperationFactory;

        public QueryStringFilterItemsParser(IFilterOperationFactory filterOperationFactory)
        {
            _filterOperationFactory = filterOperationFactory ??
                                      throw new ArgumentNullException(nameof(filterOperationFactory));
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

        public Filter[] ParseFilters(IDictionary<string, string[]> queryStringParameters)
        {
            return queryStringParameters.Where(param => param.Key.StartsWith("filter["))
                .Select(param =>
                {
                    (string[] fieldKeys, string operation) = ParseFilterItem(param.Key);
                    IFilterOperation filterOperation = ParseOperation(operation);
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

        private IFilterOperation ParseOperation(string stringOperation)
        {
            IFilterOperation operation = _filterOperationFactory.GetByShorthandName(stringOperation);
            if (operation == null)
            {
                throw new NotSupportedException($"Filter operation {stringOperation} is not supported!");
            }

            return operation;
        }
    }
}