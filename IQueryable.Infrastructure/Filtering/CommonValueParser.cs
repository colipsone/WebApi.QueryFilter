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

        public object Parse(string input)
        {
            return _typeConverter.ConvertFromString(input);
        }
    }
}