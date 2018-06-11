namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilter
    {
        IFilterOperation Operation { get; }
        string PropertyName { get; }
        string Value { get; }
    }

    public class Filter : IFilter
    {
        public IFilterOperation Operation { get; internal set; }

        public string PropertyName { get; internal set; }

        public string Value { get; internal set; }
    }

    public interface IFilterGroup   
    {
        IFilterOperation Operation { get; set; }

        IFilterGroup[] Filters { get; set; } 
    }
}