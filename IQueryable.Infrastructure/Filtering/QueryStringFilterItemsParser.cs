using System;
using System.Collections.Generic;
using System.Linq;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public class QueryStringFilterItemsParser : IFilterItemsParser
    {
        private const string FilterPrefix = "filter[";
        private static readonly char[] FilterItemSplitters = { '[', ']' };

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

        private Filter[] ParseFilterItem(KeyValuePair<string, string[]> input)
        {
            string[] values = input.Key.Split(FilterItemSplitters).Where(item => !string.IsNullOrEmpty(item)).ToArray();
            string fieldName = string.Join('.', values.Skip(1).Take(values.Length - 2));
            IFilterOperation filterOperation = ParseOperation(values.Last());
            return input.Value.Select(param => new Filter
            {
                Operation = filterOperation,
                PropertyName = fieldName,
                Value = param
            }).ToArray();
        }

        public Filter[] ParseFilters(IDictionary<string, string[]> queryStringParameters)
        {
            return queryStringParameters.Where(param => param.Key.StartsWith(FilterPrefix))
                .Select(ParseFilterItem).SelectMany(res => res).ToArray();
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