namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface IValueParser
    {
        object Parse(string input);
    }
}