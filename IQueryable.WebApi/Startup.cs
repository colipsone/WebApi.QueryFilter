using Autofac;
using IQueryableFilter.Infrastructure.Ioc;
using IQueryableFilter.WebApi.Binders;
using IQueryableFilter.WebApi.Ioc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IQueryableFilter.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options => options.ModelBinderProviders.Insert(0, new FilteringBinderProvider()));
        }
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterModule(new InfrastructureModule());
            builder.RegisterModule(new WebApiModule());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseMvc();
        }
    }
}