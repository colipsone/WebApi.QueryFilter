using System.Reflection;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface IFilterValueParserFactory
    {
        IValueParser GetValueParser(PropertyInfo property);
    }
}