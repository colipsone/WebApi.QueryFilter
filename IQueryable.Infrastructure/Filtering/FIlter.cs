namespace IQueryableFilter.Infrastructure.Filtering
{
    public class Filter
    {
        public IFilterOperation Operation { get; internal set; }

        public string PropertyName { get; internal set; }

        public string PropertyNameSource { get; internal set; }

        public string Value { get; internal set; }
    }
}