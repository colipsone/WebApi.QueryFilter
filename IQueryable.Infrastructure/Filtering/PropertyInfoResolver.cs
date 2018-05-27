using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class PropertyInfoResolver : IPropertyInfoResolver
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyInfos = new ConcurrentDictionary<string, PropertyInfo>();

        public PropertyInfo GetPropertyInfo<TEntity>(string propertyName)
        {
            return PropertyInfos.GetOrAdd(propertyName,
                name => GetFieldPropertyInfo(propertyName.Split('.'), typeof(TEntity).GetProperties()));
        }

        private static PropertyInfo GetFieldPropertyInfo(IReadOnlyList<string> propKeys, IEnumerable<PropertyInfo> properties)
        {
            PropertyInfo result = properties.FirstOrDefault(prop => prop.Name == propKeys[0]);
            if (result == null)
            {
                return null;
            }

            return propKeys.Count > 1
                ? GetFieldPropertyInfo(propKeys.Skip(1).ToArray(), result.PropertyType.GetProperties())
                : result;
        }
    }
}