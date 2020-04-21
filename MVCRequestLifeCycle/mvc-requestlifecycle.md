## Step through the MVC 3.x pipeline

### The Request Life Cycle

Request - Middleware (building blocks of the HTTP Pipeline) - Routing - Controller (Controller Factory and Controller Action Invoker)Initialization - Action Method Execution (Model Binding, Action Filters etc) - Result Execution (Result Filters, Invoke Action Result) - (Data Result || View Rendering) - Reponse (the 'request' then runs 'backwards' through the MW on it's return journey to the client, as a reponse)


### Looking at the MVC source code

https://github.com/dotnet/aspnetcore

`git tag` and 'git checkout <tag>'

Run `InstallVisualStudio.ps1` script in AspNetCore\eng\scripts using PS as admin.

Run `restore.cmd` in root

'Shift-Right Click -> Copy as Path' the Mvc.Sln in E:\Wunderpus\WLD-GitHub\AspNetCore\src\Mvc

run `startvs.cmd  < paste path here >` - launvhes VS. Build = we are good.

### Middleware

The series of components that form the application request pipeline.

Routing, Session, CORS, Authentication, Caching are all provided by Middleware.

eg: Static Files Middleware: if the request is for a static file, it will short-circuit the request ans supply the reponse (the static file) without the request progressing further along the pipeline.


Middleware will: Generate a response || perform work on the request || reroute the request

.NET provides helper config methods Run() | Use() | Map() to register components for each of the options above.

```
app.Run(async context => {
    await context.Response.WriteAsync("I am middleware")
}); //note: Run() will terminate the pipeline
```

```
app.Use(async (context, next) => {
    //logic before middleware here 

    context.Items.Add("my-key","my value"); //can be retrieved by later components

    await next.Invoke();
    //logic after middleware here
}); 
```

```
app.Map("/hello-world", SayHello); 
// if incoming requests ends in hello-world, execute the SayHello method.

private static void SayHello(IApplicationBuilder app){ 
   app.Run(async context => {
        var myVal = context.Items["my-key"];
        await context.Response.WriteAsync("I am middleware")
    });     
}
```

The above is *in-line middleware* but in reality most middleware is implemented as a standalone class.

```
public class HelloMW {
    private RequestDelegate _next;
    
    // this is called by .NET which presumably manages the list of components.
    public HelloMW(RequestDelegate next){
        _next = next
    }

    // can inject services into this method by including herem eg: IConfiguration config
    public Task Invoke(HttoContext context){
        //logic here

        await _next.Invoke(context);
    }
}
```


### Program (entry point) and Startup (config middleware)

Note: in AspNetCore\src\Mvc\samples\MvcSandbox Program and Startup files ar combined into Startup.cs (for some reason)

`Main(string[] args)` kicks things off to create a WebHostBuilder then .Run() the app.

In `CreateWebHostBuilder` some config is set, and `.UseStartup<Startup>` defines which class to use to build the MW pipeline.

##### Startup

`ConfigureServices(IServiceCollection services)` sets up the services used by the application through DI

`Configure(IApplicationBuilder app)` established the core http pipeline by registering MW components. eg: `app.UseMiddleware<MyMWclass>();`