using Autofac;
using IQueryableFilter.Infrastructure.Caching;
using IQueryableFilter.Infrastructure.Filtering;
using IQueryableFilter.Infrastructure.Time;

namespace IQueryableFilter.Infrastructure.Ioc
{
    public sealed class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // Configuration
            builder.RegisterType<LocalTimeService>().As<ITimeService>().SingleInstance();

            // Caching
            builder.RegisterType<MemoryCacheManager>().As<ICacheManager>().SingleInstance();

            // Filtering 
            builder.RegisterType<FilterExpressionBuilder>().As<IFilterExpressionBuilder>().SingleInstance();
            builder.RegisterType<NamedFilterPredicateFactory>().As<INamedFilterPredicateFactory>().SingleInstance();
            builder.RegisterType<FilterValueParserFactory>().As<IFilterValueParserFactory>().SingleInstance();
            builder.RegisterType<PropertyInfoResolver>().As<IPropertyInfoResolver>().SingleInstance();
        }
    }
}