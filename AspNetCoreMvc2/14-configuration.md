## 14-Configuration
`Program` and 'Startup' (Program runs first. It's the entry point) both handle configuration. 
Allows applications to be tailored to their environment 
(Program.Main calls BuildWebHost, which is responsible for configuring ASP.NET Core)

`Startup` creates services and middleware components (which are used to handle HTTP requests, much like the old OWIN pipeline)

The most important configuration file is \<projectname\>.csproj, which replaces the project.json file used in earlier versions of ASP.NET Core.
The csproj file is used to configure the MSBuild tool

The BuildWebHost method uses static methods defined by the WebHost class to configure ASP.NET Core. 
With the release of ASP.NET Core 2, the configuration is simplified by the use of the `CreateDefaultBuilder` method, 
which configures ASP.NET Core using settings that are likely to suit most projects. 

The UseStartup method is called to identify the class that will provide application-specific configuration; 
the convention is to use a class called Startup. 

The `Build()` method processes all the configuration settings and creates an object that implements the IWebHost interface, 

`.Run()` starts handling HTTP requests.

`CreateDefaultBuilder` is convenient, but hides details that are worth knowing about. 

See BuilWebHostManually() in SprtsStore.Program (it's not called, it's just an example)

#### Startup
##### ConfigureServices

When the application starts, ASP.NET Core creates a new instance of the Startup class and calls its `ConfigureServices` method so that the application can create its services. 

eg: first create a class UptimeService, then in `ConfigureServices` add `services.AddSingleton<UptimeService>();`

Controllers (and anything else I guess?) get free DI from services added to the IServiceCollection .


##### Configure
Next, ASP.NET calls the `Configure` method to set up the request pipeline, 
which is the set of components—known as middleware— that are used to handle incoming HTTP requests and produce responses for them.

Next ASP.NET is ready to start handling Requests.

##### Built in MVC Services and ASP.NET Middleware

eg: `services.AddMvc()`

Middleware is the term used for the components that are combined to form the request pipeline.  T
he request pipeline is arranged like a chain, and when a new request arrives, 
it is passed to the first middleware component in the chain. 
This component inspects the request and decides whether to handle it and generate a response or 
to pass it to the next component in the chain. 
Once a request has been handled, the response that will be returned to the client is passed back along the chain, 
which allows all of the earlier components to inspect or modify it (request goes through the pipeline, hits the "backstop" then back the other way and out to the client)


##### Content-Generating Middleware

Such as MVC. See `SportsStore.Infrastructure.ContentMiddleware` which is registered in Configure() 
as `app.UseMiddleware<ContentMiddleware>(); `










