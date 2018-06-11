using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IQueryableFilter.Infrastructure.Filtering.Operations
{
    internal abstract class RegularFilterOperationBase
    {
        private readonly IFilterValueParserFactory _filterValueParserFactory;
        private readonly IPropertyInfoResolver _propertyInfoResolver;

        protected RegularFilterOperationBase(IFilterValueParserFactory filterValueParserFactory, 
            IPropertyInfoResolver propertyInfoResolver)
        {
            _filterValueParserFactory = filterValueParserFactory;
            _propertyInfoResolver = propertyInfoResolver;
        }

        public string FieldName { get; set; }

        public string Value { get; set; }

        private Expression GetFilterExpression<TEntity>(Expression entityParam)
            where TEntity : class, new()
        {
            PropertyInfo propertyInfo = _propertyInfoResolver.GetPropertyInfo<TEntity>(FieldName);

            if (propertyInfo == null)
                throw new ArgumentException($"Filtered entity has no field with name {FieldName}.");

            IValueParser valueParser = _filterValueParserFactory.GetValueParser(propertyInfo);

            if (!valueParser.TryParse(Value, out object parsedValue))
            {
                throw new ArgumentException(
                    $"Inappropriate filter value '{Value}' for field with name {FieldName}.");
            }

            return GetOperationExpression(Expression.Property(entityParam, propertyInfo.Name), Expression.Constant(
                parsedValue,
                propertyInfo.PropertyType));
        }

        public abstract Expression GetOperationExpression(Expression left, Expression right);
    }
}