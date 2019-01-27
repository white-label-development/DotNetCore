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
            services.AddTransient<IOrderRepository, EFOrderRepository>();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:SportStoreProducts"]));

            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp)); //scoped to the same http request.
                                                                     //The expression receives the collection of services that have been registered and passes the collection to the GetCart method of the SessionCart class.
                                                                     //The result is that requests for the Cart service will be handled by creating SessionCart objects, which will serialize themselves as session data when they are modified. 

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //The service I created tells MVC to use the HttpContextAccessor class when implementations of the IHttpContextAccessor interface are required.
            //This service is required so I can access the current session in the SessionCart class GetCart, ie: ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            services.AddMvc();
            services.AddMemoryCache(); // sets up the in-memory data store
            services.AddSession(); // registers the services used to access session data
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
            app.UseSession(); // allows the session system to automatically associate requests with sessions when they arrive from the client

            app.UseMvc(routes => {

                routes.MapRoute(
                    name: null,
                    template: "{category}/Page{productPage:int}",
                    defaults: new { controller = "Product", action = "List" }
                );

                routes.MapRoute(
                    name: null,
                    template: "Page{productPage:int}",
                    defaults: new { controller = "Product", action = "List", productPage = 1 }
                );

                routes.MapRoute(
                    name: null,
                    template: "{category}",
                    defaults: new { controller = "Product", action = "List", productPage = 1 }
                );

                routes.MapRoute(
                    name: null,
                    template: "",
                    defaults: new { controller = "Product", action = "List", productPage = 1 });

                routes.MapRoute(name: "default", template: "{controller=Product}/{action=List}/{id?}");
            });


            SeedData.EnsurePopulated(app);
        }
    }
}
