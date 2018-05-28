namespace IQueryableFilter.Infrastructure.Filtering
{
    internal interface IValueParser
    {
        bool TryParse(string input, out object parsedValue);
    }
}