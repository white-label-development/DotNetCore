## Auth in Blazor

Focus is on securing the API (as client can be compromised in WASM)

#### cookies

`services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie();` // startup 

add mw right before .UseEndpoints
```
app.UseAuthentication();
app.UseAuthorization();
```

```
//login.cshtml.cs
var claims = new List<Claim>
{
    new Claim(ClaimTypes.Name, "Kevin"),
    new Claim(ClaimTypes.Email, Email),
};

var claimsIdentity = new ClaimsIdentity(
    claims,
    CookieAuthenticationDefaults.AuthenticationScheme);

await HttpContext.SignInAsync(
    CookieAuthenticationDefaults.AuthenticationScheme,
    new ClaimsPrincipal(claimsIdentity));
```

logout (clear cookie) with
`await HttpContext.SignOutAsync(CookieAuthenticationDefaultsAuthenticationScheme);`


**IPrincipal** represents the security context of the user on whose behalf the code is running, and includes one or more user identities (IIdentity). Usually only one per Principal

**IIdentity** represents the user on whose behalf the code is running. Usually only one per Principal

When a cookie is sent with a request it allows the server to know the Principal. BSS operates over SignalR. User must be associated with each connection. Cookie authentication allows the existing user credentials to automatically flow to SignalR connections.

To use the cookie security context in code we need to add a svc



`services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();` //startup. Allows code access to the HttpContext, eg: inject `HttpContextAccessor httpContextAccessor` into ctor.

Use built in 
```
<AuthorizeView> 
    <Authorized>only show content if ok </Authorized>
    <NotAuthorized> ... </NotAuthorized>
</AuthorizeView>
```

requires CascadingAuthenticationState around Router in App
```
<CascadingAuthenticationState>
    <Router AppAssembly="@typeof(Program).Assembly"> ...
</CascadingAuthenticationState>
```

and also change the RouteView to
```
<AuthorizeRouteView RouteData="@routeData" DefaultLayout="@typeof(MainLayout)">
    <NotAuthorized>
        <h1>Sorry, you're not authorized to view this page.</h1>
        <p>You may want to try logging in (as someone with the necessary authorization).</p>
    </NotAuthorized>
</AuthorizeRouteView>
```

AuthorizeView and CascadingAuthenticationState are powered by the AuthenticationStateProvider - a built in service that obtains state data from Core's HttpContext.User

In a page we can use `@attribute [Authorize]`. Components use `<AuthorizeView>`



In a Base:
```
public class EmployeeOverviewBase: ComponentBase
    {
        [CascadingParameter]
        Task<AuthenticationState> authenticationStateTask { get; set; }
...
    var authenticationState = await authenticationStateTask;
            if (authenticationState.User.Identity.Name == "Kevin"){ ... }
```

#### Protect the API

Cookies cannot be shared across domains (without workarounds). API might be on another domain. Token based auth is the way to go if the domains are different.

HttpClient in a blazor app will not auto add the cookie - need to do that manually.

### 3 Cookie based auth with ASP​.​NET Core Identity

Membership system: views, classes, login, logout etc

'Individual User Accounts; from new, or Project > Add > New Scaffolded Item > Identity (Override all). Set dbContext > [ADD]/

Note new `IdentityHostingStartup.cs`

`Add-Migration CreateIdentitySchema` and `update-database`

replace previous cookie auth with
`services.AddAuthentication("Identity.Application").AddCookie();`

logout with get from blazor by auto submitting Identity logout (hack?)
```
window.onload = function () {
        var form = document.getElementById('logoutForm'); form.submit();
    }
```

ASP.​NET Core Identity is better suited for Web Apps, but not ideal for Blazor. Ideally use an Identity Provider.


### 4 Token based auth with OAuth2/OIDC (Open ID Connect)

The identity provider (IDP) will be responsible for
 + Proof of authentication
 + Proof of authorization

Identity token - proof of id. used at client level transformed into a cookie

Access token - represents consent. authorization to blazor client app to access api in name of the user. Passed from client to api on each request.

OpenID Connect is a simple identity layer on top of (and extending) the oAuth2 protocol.

A client app (Blazor) can request an identity token (next to an access token if required). The identity token can then be used to sign in to the client application. The access token can be used to access the API.

We need an IDP = IdentityServer4 (https://identityserver4.readthedocs.io/en/latest/)

Setup a Hybrid flow.

nuget MS.AspNetCore.Authentication.OpenIdConnect middleware and configure startup 

```
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)

    .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme,
    options =>
    {
        options.Authority = "https://localhost:44333";
        options.ClientId = "bethanyspieshophr";
        options.ClientSecret = "108B7B4F-BEFC-4DD2-82E1-7F025F0F75D0";
        options.ResponseType = "code id_token";
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("bethanyspieshophrapi");
        //options.CallbackPath = ...
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;

    });
```

added new page LoginIDP (see demo) and LogourIDP. On request these razor pages contact the IDP 


#### the api
@skipped protecting the api and rest of module


### 5 Token based auth with ASP.​NET CORE Identity

### 6 Windows auth / AD

### 7 Authorization


## ASP.NET Core Blazor authentication and authorization (summarised)

https://docs.microsoft.com/en-us/aspnet/core/security/blazor/webassembly/?view=aspnetcore-3.1

WASM basically the same as any SPA = OIDC. In blazor, this means use primitives in `Microsoft.AspNetCore.Components.WebAssembly.Authentication` against a ASP.NET Core backend.

Blazor WebAssembly apps run on the client. Authorization is only used to determine which UI options to show. Since client-side checks can be modified or bypassed by a user, a Blazor WebAssembly app can't enforce authorization access rules.

The library integrates ASP.NET Core Identity with API authorization support built on top of Identity Server (other IDPs also supported). 




https://docs.microsoft.com/en-us/aspnet/core/security/blazor/server?view=aspnetcore-3.1&tabs=visual-studio


Blazor Server apps adopt a stateful data processing model, where the server and client maintain a long-lived relationship. The persistent state is maintained by a circuit, which can span connections that are also potentially long-lived. Authentication can be based on a cookie or some other bearer token.

Create project with Asp Net Core Identity

https://docs.microsoft.com/en-us/aspnet/core/security/blazor/?view=aspnetcore-3.1

he built-in `AuthenticationStateProvider` service obtains authentication state data from ASP.NET Core's HttpContext.User. This is how authentication state integrates with existing ASP.NET Core authentication mechanisms.

```
var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
 var user = authState.User;
 _claims = user.Claims;
```









ZWS >​<

