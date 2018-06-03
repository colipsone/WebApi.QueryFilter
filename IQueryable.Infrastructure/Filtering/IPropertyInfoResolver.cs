using System.Reflection;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface IPropertyInfoResolver
    {
        PropertyInfo GetPropertyInfo<TType>(string underlyingName);
    }
}