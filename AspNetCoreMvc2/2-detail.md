## 2 - Detail

#### New Tag Helpers, eg:
`<a asp-action="RsvpForm">RSVP Now</a>`

`<form method="post" asp-controller="Home" asp-action="Create"> `

`<label asp-for="Name">Your name:</label>` 

`<input class="form-control" asp-for="Population" asp-format="{0:#,###}" />` ... or via Model attributes `[DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]`           

`<select class="form-control" asp-for="Country" asp-items="ViewBag.Countries"> <option disabled selected value="">Select a Country</option></select>`

Replaces clunky html helpers (can still use html helpers tho).

see Jon Hilton's Cheat Sheet at <https://jonhilton.net/aspnet-core-forms-cheat-sheet/>

Roll your own:

```
using Microsoft.AspNetCore.Razor.TagHelpers;
namespace Cities.Infrastructure.TagHelpers {
    public class ButtonTagHelper : TagHelper {
        public string BsButtonColor { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output) {
            output.Attributes.SetAttribute("class", $"btn btn-{BsButtonColor}");        
        }   
 } 
}
```
The naming convention (ButtonTagHelper)  means that the Process method will be invoked for every button element in every view in the application.
So my html `<button type="submit" bs-button-color="danger">Add</button> ` is rendered as`<button type="submit" class="btn btn-danger">Add</button>`

The addition of the new css class will happen by default even if the button does not have a matching property atribute.
To limit the scope use attibutes on  class ButtonTagHelper `[HtmlTargetElement("button", Attributes = "bs-button-color", ParentTag = "form")]`

To widen the scope (apply Process to multiple elements (not just Button in this case)) `[HtmlTargetElement(Attributes = "bs-button-color", ParentTag = "form")]` 
= an anchor element with the matching attribute would have the css class injected.

Alternatively, apply multiple named elements attributes
```
[HtmlTargetElement("button", Attributes = "bs-button-color", ParentTag = "form")]    
[HtmlTargetElement("a", Attributes = "bs-button-color", ParentTag = "form")]
```

To work, the helper must be registered with @addTagHelper in the View ot ViewImports file 
eg: `@addTagHelper Cities.Infrastructure.TagHelpers.*, Cities`  The first part of the argument specifies the names of the tag helper classes, with support for wildcards, and the second part specifies the name of the assembly in which they are define

##### The Built-In Tag Helper Attributes for Form Elements

asp-controller, asp-action, asp-route-* (eg:asp-route-page=23), asp-route, asp-are, asp-antiforgery



#### Controller return types

Original MVC controller actions return a `IActionResult` wheras the demo returns a `ViewResult`. 

Todo: be clear about the difference

#### Validation (same so far)
" MVC supports declarative validation rules defined with attributes from the System.ComponentModel.DataAnnotations namespace, meaning that validation constraints are expressed using the standard C# attribute features"

MVC automatically detects the attributes and uses them to validate data during the model-binding process

#### Web Artifacts

 The convention is that third-party CSS and JavaScript packages are installed into the wwwroot/lib folder. Bootstrap etc..

#### Startup 

Learn more about these changes, such as ` services.AddMvc()`, `app.UseMvcWithDefaultRoute();`

`app.UseDeveloperExceptionPage();` is self explanatory.

`app.UseBrowserLink();` enables BrowserLink (which used to be awful. Revisit this?)


#### Quick Razor Notes

 You can specify a set of namespaces that should be searched for types by adding a view imports file to the project. The view imports file is placed in the Views folder and is named `_ViewImports.cshtml`

#### Managing Software Packages 

the Microsoft.AspNetCore.All package is a meta-package, which contains all the individual nuget packages required by asp .net Core and the MVC framework, which means you don’t need to add packages one by one. When you publish your application, any individual packages that are part of the meta-package but not used by the application will be removed, ensuring that you don’t deploy more packages that you need.
The NuGet tool keeps track of the project’s packages in the \{projectname\}.csproj file, eg:

```
<ItemGroup>  
	<PackageReference Include="Microsoft.AspNetCore.All" Version="2.0.0" /> 
</ItemGroup> 
```
 You can also edit the .csproj file directly and Visual Studio will detect changes and download and install the packages added (cool!)

Note: for Core 2.1: Replace the package reference for `Microsoft.AspNetCore.All` with a package reference for `Microsoft.AspNetCore.App`
https://docs.microsoft.com/en-us/aspnet/core/migration/20_21?view=aspnetcore-2.2


#### Bower

Bower packages are specified through the bower.json file.  the repository for Bower packages is http://bower.io/search, where you can search for packages to add to your project.


#### Static Content Delivery 

add `app.UseStaticFiles();` to the Startup class for delivering static files from the wwwroot folder to clients.

#### Bundling and Minifying

add extension:  Tools > Extensions and Updates > Bundler & Minifier

Select files in the order they should be.
As you perform bundling and minification operations, 
the extension keeps a record of the files that have been processed in a file called bundleconfig.json in the root folder of the project.

The extension automatically monitors the input files for changes and regenerates the output files when there are changes, ensuring that any edits you make are reflected in the bundled and minified files. 
(versioning ??? v1.1.2 etc)


#### Controller Action Results

`IActionResult` (return anthing that implements the interface)

`ViewResult` (the action result that provides access to the Razor view engine, which processes .cshtml files to incorporate model data and sends the result to the client through the HttpResponse context engine)

##### The Content Action Results
JsonResult `public JsonResult Index() => Json(new[] { "Alice", "Bob", "Joe" });`

ContentResult ( This action result sends a response whose body contains a specified object. string ann optional meme type )
`public ContentResult Index()  => Content("[\"Alice\",\"Bob\",\"Joe\"]", "application/json")` No content negotiation means the client might not cope.

ObjectResult ( This action result will use content negotiation to send an object to the client.) 

OkObjectResult ( send an object to the client with an HTTP 200 status code if the content negotiation is successful. )
`public ObjectResult Index() => Ok(new string[] { "Alice", "Bob", "Joe" });`

NotFoundObjectResult (send an object to the client with an HTTP 404 status code if the content negotiation is successful.)

Also

FileContentResult (byte[], mime type)

FileStreamResult  (read stream and send content)

VirtualFileResult (read stream from a virtual path) `public VirtualFileResult Index() => File("/lib/bootstrap/dist/css/bootstrap.css", "text/css");`

PhysicalFileResult (read file and send content)

..and Status Code Action Results (StatusCodeResult, OkResult, NotFoundResult  etc..  )

####  Service Life Cycles 

##### `AddTransient<service, implType>()`
This method tells the service provider to create a new instance of the implementation type for every dependency on the service type

“Using the Transient Life Cycle: `AddTransient`  tells the service provider to create a new instance of the implementation type whenever it needs to resolve a dependency.
 The transient life cycle incurs the cost of creating a new instance of the implementation class every time a dependency is resolved, 
but the advantage is that you don’t have to worry about managing concurrent access or ensure that objects can be safely reused for multiple requests.






##### `AddTransient<service>()`
This method is used to register a single type, which will be instantiated for every dependency

Using Dependency Injection for Concrete Types:

##### `AddTransient<service>(factoryFunc)`
This method is used to register a factory function that will be invoked to create an implementation object for every dependency on the service type

Using a Factory Function:
One version of the AddTransient method accepts a factory function that is invoked every time there is a dependency on the service type. 
This allows the object that is created to be varied so that different dependencies receive instances of different types or instances that are configured differently.

eg:
```
services.AddTransient<IRepository>(provider => { 
    if (env.IsDevelopment()) {
        var x = provider.GetService<MemoryRepository>();                    
        return x;                
    } else {                    
        return new AlternateRepository();                
    }            
});
```

The expression receives a System.IServiceProvider object, which can be used to create instances of other types that have been registered with the service provider.

Note the use of GetService to create the object -  because MemoryRepository has its own dependency on the IModelStorage interface and using the service provider to create the object means that detecting and resolving the dependency will be managed automatically
— but it does mean I have to specify the life cycle that should be used for MemoryRepository objects, like this:
`services.AddTransient<MemoryRepository>();` Without this statement, the service provider would not have the information it needs to create and manage MemoryRepository objects.

AlternateRepository can be created directly using the new keyword because it doesn’t declare any dependencies in its constructor.




##### `AddScoped<service, implType>(); AddScoped<service>(); AddScoped<service>(factoryFunc);`
These methods tell the service provider to reuse instances of the implementation type so that all service requests made by components associated with a common scope, which is usually a single HTTP request, share the same object. These methods follow the same pattern as the corresponding AddTransient method

Using the Scoped Life Cycle:
This life cycle creates a single object from the implementation class that is used to resolve all of the dependencies associated with a single scope, 
which generally means a single HTTP request. Since the default scope is the HTTP request, this life cycle allows for a single object to be shared by all the components that process a request and is most often used for sharing common context data when writing custom classes, such as routes


##### Action injection

eg: `public ViewResult Index([FromServices]ProductTotalizer totalizer) { ... }`

Is useful when you have a dependency on an object that is expensive to create and that is required in only one of the action methods defined by a controller.



##### manual "injection"

`IRepository repository = HttpContext.RequestServices.GetService<IRepository>()`
Service locator pattern - for when injection via startup can't be used.

#### Filters

for cross-cutting concerns (logging, authorization, caching etc).

Global filters are applied once in the Startup class and, as their name suggests, are automatically applied to every action method in every controller in the application


#### API Controllers

Now integrated into ASP.NET Core MVC. JSON by default. XML via SrartUp ` services.AddMvc().AddXmlDataContractSerializerFormatters`

`patch.ApplyTo(res);` ?? ApplyTo ??

If you return null from an apI controller action method, then the client will be sent a 204 – No Content response. 

Can use powershell to test `Invoke-RestMethod http://localhost:7000/api/reservation -Method GET`

and `Invoke-RestMethod http://localhost:7000/api/reservation -Method POST -Body  (@{clientName="Anne"; location="Meeting Room 4"} | ConvertTo-Json) -ContentType "application/json"`

The Patch Format is used for patches. Here ClientName and Location are being patched:

`Invoke-RestMethod http://localhost:7000/api/reservation/2 -Method PATCH -Body (@ { op="replace"; path="clientName"; value="Bob"},@{ op="replace"; path="location"; value="Lecture Hall"} | ConvertTo-Json)  -ContentType "application/json"`


```
[Produces("application/json")]        
public Reservation GetObject() => ... //override the content negotiation system and specify a data format directly 
```

and

```
[HttpPost]        
[Consumes("application/xml")]        
public Reservation ReceiveXml([FromBody] Reservation reservation) { ... }
```

#### Views


Useful RazorPage\<T\> Properties for View Development :

Model, ViewData, ViewCOntext, Layout, ViewBag, TempData, Context, User, RenderSection(), RenderBody(), IsSectionDefined()

The Razor Helper Properties : HtmlEncoder, Component, Json, Url, Html

##### View Components

Replace child action feature.  View components are classes that provide action-style logic to support partial views (by providing data).
Used for repeated, embedded fnuctionality lick a shopping basket summary or login/auth panel - this "data" might be required on a page whose main function is something else.
Without components, that pages' view model would need to contain the additional "unrelated" data.

A POCO view component (doesn't use MVC APIs) is any class whose name ends with ViewComponent and that defines an Invoke method `public string Invoke(){...}`. 
Placed in /Components. `@await Component.InvokeAsync(nameof(myClass)`

Normally it makes sense to not use a POCO, but derive from ViewComponent base. 
You don’t need to include ViewComponent in the class name when you derive from the base class. 
Instead of Invoke just returning a string, consider ViewViewComponentResult (return a view), 
ContentViewComponentResult (return encoded text), HtmlContentViewComponentResult  (return a HTML fragment).

Also InvokeAsync. Component logic can go into a normal controller
```
[ViewComponent(Name = "ComboComponent")]   
public class CityController : Controller { .. }
```
and `  @await Component.InvokeAsync("ComboComponent")`








