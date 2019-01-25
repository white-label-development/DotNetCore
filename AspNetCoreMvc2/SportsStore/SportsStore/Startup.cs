using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Models;

namespace SportsStore
{
    public class Startup
    {
        public IConfiguration Configuration { get; } //receives the configuration data loaded from the appsettings.json file, which is presented through an object that implements the IConfiguration interface. 

        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // when a component, such as a controller, needs an implementation of the IProductRepository interface, it should receive an instance of the FakeProductRepository class. T
            // The AddTransient method specifies that a new FakeProductRepository object should be created each time the IProductRepository interface is needed
            //services.AddTransient<IProductRepository, FakeProductRepository>(); //drop the fake, we are using EF now..
            services.AddTransient<IProductRepository, EFProductRepository>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:SportStoreProducts"]));
                            
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // The Configure method is used to set up the features that receive and process HTTP requests.
        // Each method that I call in the Configure method is an extension method that sets up an HTTP request processor
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.Run(async (context) =>
            //{
            //    await context.Response.WriteAsync("Hello World!");
            //});

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseMvc(routes => {
                routes.MapRoute(name: "default", template: "{controller=Product}/{action=List}/{id?}");
            });
        }
    }
}
