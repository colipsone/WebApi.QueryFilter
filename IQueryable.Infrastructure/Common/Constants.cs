namespace IQueryableFilter.Infrastructure.Common
{
    public static class Constants
    {
        public static class CacheKaeys
        {
            private const string WebAppPrefix = "Web:";

            private const string QueryFiltersTemplate = "IQueryableFilter:";

            public const string FilterSetUniqueNameTemplate = WebAppPrefix + QueryFiltersTemplate + "FilterSetUniqueName:";

            public const string FilterPropertyNameTemplate = WebAppPrefix + QueryFiltersTemplate + "FilterPropertyName:";
        }
    }
}