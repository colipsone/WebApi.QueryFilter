using System.Collections.Generic;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilterItemsParser
    {
        Filter[] ParseFilters(IDictionary<string, string[]> queryStringParameters);
    }
}