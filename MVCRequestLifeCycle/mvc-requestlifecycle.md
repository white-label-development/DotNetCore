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

### 3 Middleware

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

##### Program

`Main(string[] args)` kicks things off to create a WebHostBuilder then .Run() the app.

In `CreateWebHostBuilder` some config is set, and `.UseStartup<Startup>` defines which class to use to build the MW pipeline.

##### Startup

`ConfigureServices(IServiceCollection services)` sets up the services used by the application through DI

`Configure(IApplicationBuilder app)` established the core http pipeline by registering MW components. eg: `app.UseMiddleware<MyMWclass>();`


### 4 Routing

Attribute routing cancels out Conventional routing.

#### Endpoint Routing

Legacy routing pipeline had problems: Middleware could not know which Action Method will be selcted (by the MVC Route Handler which was after the MW pipeline). Also too coupled to MVC.

Endpoint Routing extracts routing OUT of MVC and INTO the MW pipeline. 2 parts:

**Endpoint Routing Middleware** - decides which Endpoint should receive the request

**Endpoint Middleware** - Executes the selected endpoint to generate a response.

An Endpoint is a class that contains a Request Delegate and other metadata used to generate some kind of response. For MVC the delegate would be a wrapper around Controller and Action Method execution. The delegate is the bridge/pipeline between MW and MVC (or other parts)

```
public class RouteEndpoint: Endpoint{
    public string DisplayName {get;set;} // friendly name
    public string Pattern {get;set;} // route pattern for matching
    public EndpointMetadata {get;} // misc metadata
    public RequestDelegate RequestDelegate {get;} // the delegate to generate a response
}
```

### 5 Controller Initialization

`ResourceInvoker`, `ControllerActionInvoker` - orchestrate the MVC Pipeline through Invoke

`State.ActionBegin`

#### Filters

Authorization Filters execute first. Can short circuit the life cycle for unathourized requests.

Note use of TypeFilter (or ServiceFilter) to enable ctor dependency of filter to be injected, eg:

`public OutageAuthorizationFilter(IConfiguration configuration)` ctor
```
[TypeFilter(typeof(OutageAuthorizationFilter))]
public class HomeController ...
```

Of note: 

ResourceFilter: OnResourceExecuting (very early in mvc pipeline) and OnResourceExecuted() very late. Useful for caching, short-circuiting the request, model binding modification etc

MiddlewareFilter: Allows re-use of a middleware component in the actual MVC life cycle

Summary:
The Endpoint Request Delegate executes the Controller Action Invoker (which inhertis from Resource Invoker). These invokers orchestrate the MVC Pipeline. AuthorizationFilters execute first in the MVC pipeline. ResoueceFilters execute logic at the start and end of the MVC pipeline.

A controller instance is retrieved using the Controller Factory.

### 6 Controller Action Methods

Handle incoming requests and generate Action Results. Rendering is abstracted out to Action **Result** objects wgich define the data and type of response to render.


#### Controller Action Invoker

Model Binding runs in `State.ActionBegin` =  early.
The Action method is run from `State.ActionInside` via `InvokeActionMethodAsyc()`

#### Model Binding

Model Binder Factory will try to find a Provider to bind the model and values

### 7 Action Results and the View Engine

Vague IActionResult fulfilled with View() -> ViewResult:ActionResult Content() etc

Action Results are created by Action Methods and executed by the Resource Invoker logic.

Action Results render the response data back to the client in different formats

Result Filters inject logic before and after Action Result execution

View Results trigger the Razor View Engine rendering process.
 + The View Engine locates the View to render
 + The View Engine Result communicates the result of that search
 + THe View renders markup to the response
