using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace SportsStore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseDefaultServiceProvider(options => options.ValidateScopes = false); //not sure about ValidateScopes. Research...

        public static IWebHost BuilWebHostManually(string[] args)
        {

            //useKestrel = configures the ()cross-platform) Kestrel web server
            //ContentRoot = is used for loading configuration files and delivering static content such as images, JavaScript, and CSS. 
            //ConfigureAppConfiguration  =  prepare the configuration data for the application
            //AddUserSecrets = see https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-2.2&tabs=windows
            // ...
            //UseDefaultServiceProvide = his method is used to configure dependency injection

            return new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())

                .ConfigureAppConfiguration((hostingContext, config) => {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("appsettings.json",
                            optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json",
                            optional: true, reloadOnChange: true);
                    config.AddEnvironmentVariables();
                    if (args != null)
                    {
                        config.AddCommandLine(args);
                    }
                })

                .ConfigureLogging((hostingContext, logging) => {
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddConsole();
                    logging.AddDebug();
                })

                .UseIISIntegration()
                .UseDefaultServiceProvider((context, options) => {
                    options.ValidateScopes =
                        context.HostingEnvironment.IsDevelopment();
                })
                .UseStartup<Startup>()
                .Build();
        }

    }
}
