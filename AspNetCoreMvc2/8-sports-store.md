### 8- SportsStore "sing along"

#### Setup

Built in DI via Startup is new. eg: `services.AddTransient<IProductRepository, FakeProductRepository>()`

Startup routes eg: `routes.MapRoute(  name: "default", template: "{controller=Product}/{action=List}/{id?}");`

"The UseMvc method sets up the MVC middleware, and one of the configuration options is the scheme that will be used to map URLs to controllers and action methods."


#### Tag Helpers

```
[HtmlAttributeName(DictionaryAttributePrefix = "page-url-")]
public Dictionary<string, object> PageUrlValues { get; set; } = new Dictionary<string, object>();
```

Decorating a tag helper property with the HtmlAttributeName attribute allows me to specify a prefix for attribute names on the element, 
which in this case will be page-url-. 
The value of any attribute whose name begins with this prefix will be added to the dictionary that is assigned to the PageUrlValues property, 
which is then passed to the IUrlHelper.Action method to generate the URL for the href attribute of the a elements that the tag helper produces. 

`<div page-url-category="@Model.CurrentCategory" ...`

#### Services

"Services are most commonly used to hide details of how interfaces are implemented from the components that depend on them"

see `ConfigureServices`, 

eg: `services.AddScoped<Cart>(sp => SessionCart.GetCart(sp));`

allows a SessionCare to be injected when a Cart is requested:
```
public CartController(IProductRepository repo, Cart cartService)
{
    repository = repo;
    cart = cartService;
}
```

#### Security

Authentication and authorization are provided by the ASP.NET Core Identity system.


#### Deployment

New Az sub. Free Trial.

Added `appsettings.production.json` 

from PowerShell:
```
> $env:ASPNETCORE_ENVIRONMENT="Production"
> dotnet ef database update --context ApplicationDbContext 
> dotnet ef database update --context AppIdentityDbContext
```
