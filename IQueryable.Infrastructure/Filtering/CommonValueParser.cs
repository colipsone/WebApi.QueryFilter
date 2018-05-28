using System;
using System.ComponentModel;

namespace IQueryableFilter.Infrastructure.Filtering
{
    public class CommonValueParser : IValueParser
    {
        private readonly TypeConverter _typeConverter;

        public CommonValueParser(TypeConverter typeConverter)
        {
            _typeConverter = typeConverter ?? throw new ArgumentNullException(nameof(typeConverter));
        }

        public bool TryParse(string input, out object parsedValue)
        {
            if (!_typeConverter.IsValid(input))
            {
                parsedValue = null;
                return false;
            }

            parsedValue = _typeConverter.ConvertFromString(input);
            return true;
        }
    }
}