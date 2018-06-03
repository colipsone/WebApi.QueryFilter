using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class PropertyInfoResolver : IPropertyInfoResolver
    {
        private static readonly ConcurrentDictionary<string, PropertyInfo> PropertyInfos =
            new ConcurrentDictionary<string, PropertyInfo>();

        private static readonly ConcurrentDictionary<Type, IDictionary<string, string>> TypePropertyNameMapCache =
            new ConcurrentDictionary<Type, IDictionary<string, string>>();

        private readonly IContractResolver _contractResolver;

        public PropertyInfoResolver(IContractResolver contractResolver)
        {
            _contractResolver = contractResolver ?? throw new ArgumentNullException(nameof(contractResolver));
        }

        public PropertyInfo GetPropertyInfo<TEntity>(string underlyingName)
        {
            if (underlyingName == null) throw new ArgumentNullException(nameof(underlyingName));

            return PropertyInfos.GetOrAdd(underlyingName,
                name => GetFieldPropertyInfo(name.Split('.'), typeof(TEntity)));
        }

        private PropertyInfo GetFieldPropertyInfo(IReadOnlyList<string> propKeys, Type type)
        {
            while (true)
            {
                string propSourceName = GetPropSourceName(propKeys[0], type);
                if (string.IsNullOrEmpty(propSourceName)) return null;

                PropertyInfo result = type.GetProperties().FirstOrDefault(prop => prop.Name == propSourceName);
                if (result == null) return null;

                if (propKeys.Count <= 1) return result;
                propKeys = propKeys.Skip(1).ToArray();
                type = result.PropertyType;
            }
        }

        private string GetPropSourceName(string key, Type type)
        {
            IDictionary<string, string> map = TypePropertyNameMapCache.GetOrAdd(type,
                t =>
                {
                    if (!(_contractResolver.ResolveContract(t) is JsonObjectContract contract)) return null;

                    return contract.Properties.ToDictionary(p => p.PropertyName, p => p.UnderlyingName,
                        StringComparer.OrdinalIgnoreCase);
                });

            return !map.TryGetValue(key, out string name) ? null : name;
        }
    }
}