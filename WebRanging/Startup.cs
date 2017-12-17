using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebRanging.Daemons;

namespace WebRanging
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var config = Configuration.Get<Config>();
            services.AddMvc();
            var builder = new ContainerBuilder();
            builder.Populate(services);

            builder.RegisterAssemblyTypes(GetType().Assembly)
                .Where(t => !typeof(Controller).IsAssignableFrom(t))
                .Where(t => !typeof(IDaemon).IsAssignableFrom(t))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register<IDbContext>(ctx =>
                new DbContext(config.Mongo.ConnectionString, config.Mongo.DatabaseName));
            builder.Register<IConfigProvider>(ctx =>
                new ConfigProvider(config.Application.StoreSitesFolder));

            return new AutofacServiceProvider(builder.Build());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}