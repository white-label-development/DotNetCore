using InvoiceManagementApp.Application;
using InvoiceManagementApp.Application.Common.Interfaces;
using InvoiceManagementApp.Infrastructure;
using InvoiceManagementApp.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;




namespace InvoiceManagementApp
{
    public static class ApiConfigurationConsts
    {
        public const string ApiName = "Clean Architecture API";
        public const string ApiVersionV1 = "v1";
        public const string OpenApiContactName = "NT";
        public const string OpenApiContactEmail = "neil@";
        public const string OpenApiContactUrl = "https://www.test.com";
    }

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
            services.AddInfrastructure(Configuration);
            services.AddApplication();

            services.AddScoped<ICurrentUserService, CurrentUserService>();

            services.AddControllersWithViews();
            services.AddRazorPages();



            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });

            //NT: Looks like the nswag version replaces / overrides this. Can;t we have both? Needs more investigation...
            //services.AddSwaggerGen(options =>
            //{
            //    // https://docs.microsoft.com/en-us/aspnet/core/tutorials/getting-started-with-swashbuckle?view=aspnetcore-3.1&tabs=visual-studio#customize-and-extend

            //    options.SwaggerDoc(
            //        ApiConfigurationConsts.ApiVersionV1,

            //        new OpenApiInfo
            //        {
            //            Title = ApiConfigurationConsts.ApiName + "A",
            //            Version = ApiConfigurationConsts.ApiVersionV1,
            //            Contact = new OpenApiContact
            //            {
            //                Name = ApiConfigurationConsts.OpenApiContactName,
            //                Email = ApiConfigurationConsts.OpenApiContactEmail,
            //                Url = new Uri(ApiConfigurationConsts.OpenApiContactUrl),
            //            }
            //        });

            //    //NT does not work atm
            //    // Set the comments path for the Swagger JSON and UI - previously generated XML - csproj XML -> GenerateDocumentationFile
            //    //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            //    // xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            //    //options.IncludeXmlComments(xmlPath);
            //});

            //nswag
            services.AddOpenApiDocument(configure =>
            {                
                configure.Title = ApiConfigurationConsts.ApiName + "B";
                configure.AddSecurity("JWT", Enumerable.Empty<string>(), new NSwag.OpenApiSecurityScheme
                {
                    Type = NSwag.OpenApiSecuritySchemeType.ApiKey,
                    Name = "Authorization",
                    In = NSwag.OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                configure.OperationProcessors.Add(new NSwag.Generation.Processors.Security.AspNetCoreOperationSecurityScopeProcessor("JWT"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();

            


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            //Microsoft.AspNetCore.Builder.SwaggerBuilderExtensions
            //app.UseSwagger();  // Enable middleware to serve generated Swagger as a JSON endpoint.

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", ApiConfigurationConsts.ApiName + "C");
                //c.RoutePrefix = string.Empty; //Configures swagger to load at application root
            });

            //nswag
            app.UseOpenApi();
            app.UseSwaggerUi3();

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });

            
        }
    }
}
