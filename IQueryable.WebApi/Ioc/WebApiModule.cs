using Autofac;
using IQueryableFilter.Infrastructure.Filtering;
using IQueryableFilter.WebApi.Filtering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Serialization;

namespace IQueryableFilter.WebApi.Ioc
{
    public class WebApiModule: Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            builder.RegisterType<HttpContextAccessor>().As<IHttpContextAccessor>().SingleInstance();

            builder.Register((c, p) => (IContractResolver) c.Resolve<IHttpContextAccessor>().HttpContext.RequestServices
                .GetService(typeof(DefaultContractResolver)));

            builder.Register(c => new TestEntityNamedFilter())
                .As<INamedFilter>()
                .WithMetadata("FilterName", "test_named");
        }
    }
}