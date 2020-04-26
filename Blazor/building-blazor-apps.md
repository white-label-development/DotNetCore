## Building Blazor Apps
https://github.com/alex-wolf-ps/blazor-enterprise

### Web Assembly (preview) | Server (1.0)

2 hosting models for the same code.

Server version uses a SignalR library and websockets to provide the illusion of client hosting.

Blazor Design Patterns: Component driven. Background http requests.



### DI and Application State

@inject || [Inject]

##### Provided Services

See ConfigureServices

```
[Inject]
public ILogger<MyClass> Logger {get;set;} // default logger writes to console
```

##### Service Lifetime

+ AddTransient - new instance each time it's needed
+ AddSingleton
+ AddScoped - created and reused for each Blazor circuit

A Blazor circuit is an abstraction over the SignalR Connection between the browser and server to manage state and scope (what?).

I think this means if the same instance is used across multiple components in a "circuit" (like a session maybe) each component does not get a fresh instance (like in transient) but the same one.

### 4 Enhancing the App

routing will first look for MVC and Razor routes, before passing to _host

Currently optional route parameters are not supported, but multiple routes are

Built in route constraints are standard: bool, decimal, datetime, double, float, guid, int, long

NavLink, NavigationManager

Razor Class Library for distributable parts. Nuget packages.

Application State Options:

+ url - query strings
+ databases
+ browser - local storage && session storage. nuget ProtectedBrowserStorage is good. Note Host App RenderMode.Server requirement for JSInterop.


### 5 HTTP Comms with Blazor

HttpClientFactory. Typed is recommended. Can also just use "raw" HCF, eg:
```
[Inject]
public IHttpClientFactory _hcf;
...
var client = _hcf.CreateClient();
```
nuget Ms Blazor.HttpClient 




### 6 Advanced Form WorkFlows

Blazor does not handle null properties very well. Set defaults.

No Form validation for nested types - try nuget package MS.Blazor.DataAnnotations.Validation

Replace build in `<DataAnnotationsValidator />` with `<ObjectGraphDataAnnotationsValidator />` and decorate nested types in model with `[ValidateComplexType]` attribute. That's it.

We can make custom validators:

`CustomPropertyValidator: ValidationAttribute` // prop

`Employee: IValidatableObject` // model level

#### Custom Inputs

See CustomInputSelect example, that extends source for InputSelect 