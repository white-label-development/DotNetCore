﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Infrastructure;
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
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:SportStoreIdentity"]));


            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();


            services.AddScoped<Cart>(sp => SessionCart.GetCart(sp)); //scoped to the same http request.
                                                                     //The expression receives the collection of services that have been registered and passes the collection to the GetCart method of the SessionCart class.
                                                                     //The result is that requests for the Cart service will be handled by creating SessionCart objects, which will serialize themselves as session data when they are modified. 

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //The service I created tells MVC to use the HttpContextAccessor class when implementations of the IHttpContextAccessor interface are required.
            //This service is required so I can access the current session in the SessionCart class GetCart, ie: ISession session = services.GetRequiredService<IHttpContextAccessor>()?.HttpContext.Session;

            services.AddSingleton<UptimeService>();
            services.AddTransient<Neil>();

            services.AddMvc(); //  sets up every service that MVC needs without filling up the ConfigureServices method with an enormous list of individual services. Has extension methods for fine tuning configuration
            services.AddMemoryCache(); // sets up the in-memory data store
            services.AddSession(); // registers the services used to access session data
        }

        //only applies in a development environment. used INSTEAD of ConfigureServices if env matches.
        //public void ConfigureDevelopmentServices(IServiceCollection services) { services.AddSingleton<UptimeService>(); services.AddMvc(); }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // The Configure method is used to set up the features that receive and process HTTP requests.
        // Each method that I call in the Configure method is an extension method that sets up an HTTP request processor
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                // The UseDeveloperExceptionPage method sets up an error-handling middleware component that displays details of the exception in the response, including the exception trace.
                // This isn’t information that should be displayed to users
                app.UseDeveloperExceptionPage();
                app.UseStatusCodePages(); // The UseStatusCodePages method adds descriptive messages to responses that contain no content, such as 404 - Not Found response
            }
            else
            {
                // This method sets up an error handling that allows a custom error message to be displayed that won’t reveal the inner workings of the application. 
                app.UseExceptionHandler("/Error");
            }

                       
            app.UseStaticFiles();
            app.UseSession(); // allows the session system to automatically associate requests with sessions when they arrive from the client
            app.UseAuthentication(); //  set up the components that will intercept requests and responses to implement the security policy.

            app.UseMiddleware<ErrorMiddleware>(); // Response-Editing Middleware FIRST: as can only inspect middleware declared after it. anything before is unavailable.
            app.UseMiddleware<BrowserTypeMiddleware>(); // Request-Editing Middleware: set this before consumers as creates httpContext.Item that is consumed in ShortCircuitMiddleware
            app.UseMiddleware<ShortCircuitMiddleware>(); //apply shortcircuit before content (or it's too late)
            app.UseMiddleware<ContentMiddleware>(); //ch14 example
            app.UseMiddleware<ContentMiddleware2>(); //spike chained middleware. ok. returns "This is from the content middleware (uptime: 179ms)This is from ContentMiddleware2"

            //app.UseMvcWithDefaultRoute();

            //note: curly bracket part of route is termed a segment
            app.UseMvc(routes =>
            {

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

                //routes.MapRoute(
                //    name: null,
                //    template: "{product}/{",
                //    defaults: new { controller = "Product", action = "List", productPage = 1 });

                routes.MapRoute(name: "default", template: "{controller=Product}/{action=Index}/{id?}");
            });

            //if (env.IsDevelopment())
            //{
            //    SeedData.EnsurePopulated(app);
            //    IdentitySeedData.EnsurePopulated(app);
            //}
        }

        //only applies in a development environment
        //public void ConfigureDevelopment(IApplicationBuilder app, IHostingEnvironment env)
        //{
        //    // blah
        //}

        //the next level of this is rather than have env specific methods, use classes, eg: StartupDevelopment
    }


}

