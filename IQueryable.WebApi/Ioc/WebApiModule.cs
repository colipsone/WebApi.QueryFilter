using Autofac;
using IQueryableFilter.Infrastructure.Filtering;
using IQueryableFilter.WebApi.Filtering;

namespace IQueryableFilter.WebApi.Ioc
{
    public class WebApiModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.Register(c => new TestEntityNamedFilter())
                .As<INamedFilter>()
                .WithMetadata("FilterName", "test_named");
        }
    }
}