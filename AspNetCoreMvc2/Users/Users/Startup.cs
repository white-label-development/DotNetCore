using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Infrastructure;
using Users.Models;

namespace Users
{
    public class Startup
    {

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration) => Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            

            //Once registered, Each time a request is received, the claims transformation middleware calls the LocationClaimsProvider.TransformAsync method, which simulates the HR data source and creates custom claims. 
            services.AddSingleton<IClaimsTransformation, LocationClaimsProvider>();


            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:SportStoreIdentity"]));


            services.AddAuthorization(aopts =>
            {
                aopts.AddPolicy("DCUsers", policy =>
                {
                    policy.RequireRole("Users");
                    policy.RequireClaim(ClaimTypes.StateOrProvince, "DC");
                });
            });


            services.AddIdentity<AppUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                //opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyz";
                opts.Password.RequiredLength = 6;
                opts.Password.RequireNonAlphanumeric = false;
                opts.Password.RequireLowercase = false;
                opts.Password.RequireUppercase = false;
                opts.Password.RequireDigit = false;
            }).AddEntityFrameworkStores<AppIdentityDbContext>()
            .AddDefaultTokenProviders();
            // Can make  CustomPasswordValidator : IPasswordValidator<AppUser> and registering  services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordValidator>();
            // Can make  CustomUserValidator : IUserValidator<AppUser>  ...  services.AddTransient<IUserValidator<AppUser>, CustomUserValidator>();
            // The AddEntityFrameworkStores method specifies that Identity should use Entity Framework Core to store and retrieve its data, using the database context class.
            // A ddDefaultTokenProviders method uses the default configuration to support operations that require a token, such as changing a password

            //Google. Reset Secret in console.developers.google.com
            services.AddAuthentication().AddGoogle(opts => { opts.ClientId = "794564220703-mf8d7kjksqvlkm316ih64b7ocquhinjd.apps.googleusercontent.com"; opts.ClientSecret = "dG3vxfySmO3C9ksNNlwZOPWO"; });



            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStatusCodePages();
            app.UseStaticFiles();
            app.UseAuthentication(); // adds ASP.NET Core Identity to the request-handing pipeline, which allows user credentials to be associated with requests based on cookies or URL rewriting
            app.UseMvcWithDefaultRoute();

            //comment out when making migrations (sigh)
            //AppIdentityDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait();
        }
    }
}
