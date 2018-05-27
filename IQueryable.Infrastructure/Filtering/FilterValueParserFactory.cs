using System;
using System.ComponentModel;
using System.Reflection;
using IQueryableFilter.Infrastructure.Caching;
using IQueryableFilter.Infrastructure.Common;

namespace IQueryableFilter.Infrastructure.Filtering
{
    internal class FilterValueParserFactory : IFilterValueParserFactory
    {
        private readonly ICacheManager _cacheManager;

        public FilterValueParserFactory(ICacheManager cacheManager)
        {
            _cacheManager = cacheManager ?? throw new ArgumentNullException(nameof(cacheManager));
        }

        public IValueParser GetValueParser(PropertyInfo property)
        {
            return _cacheManager.GetOrAdd(Constants.CacheKaeys.FilterPropertyNameTemplate + property.PropertyType.FullName,
                () =>
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
                    return new CommonValueParser(converter);
                },
                null);
        }
    }
}