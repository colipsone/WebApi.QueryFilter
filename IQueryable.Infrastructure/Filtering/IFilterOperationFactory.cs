namespace IQueryableFilter.Infrastructure.Filtering
{
    public interface IFilterOperationFactory
    {
        IFilterOperation GetByShorthandName(string name);
    }
}