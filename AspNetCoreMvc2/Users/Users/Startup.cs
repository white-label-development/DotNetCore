using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Users.Models;

namespace Users
{
    public class Startup
    {

        public IConfiguration Configuration { get; }


        public Startup(IConfiguration configuration) =>  Configuration = configuration;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            //services.AddTransient<IPasswordValidator<AppUser>,CustomPasswordValidator>();
            //services.AddTransient<IUserValidator<AppUser>, CustomUserValidator>();

            services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:SportStoreIdentity"]));

            services.AddIdentity<AppUser, IdentityRole>(opts => {
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
