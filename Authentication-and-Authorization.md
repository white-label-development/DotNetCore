# Authentication and Authorization in .NET Core

## tl;dr;

Authoriation code flow with PKCE is recommended for all interactive clients.

Use Client Credentials flow for non-interactive server to server.

## Understanding

Authentication determines identity.

Analogy:

+ Person at hotel show passport to clerk (authentication using "passport scheme").
+ Receives limited access door key (authorization. only some "rooms" can be unlocked).

### ASP.NET Core AuthN setup

In ASP.NET Core, authentication is handled by the `IAuthenticationService`, which is used by authentication middleware. The authentication service uses registered authentication handlers to complete authentication-related actions. The  handlers and their configuration options are called __"schemes"__ (specified by registering authentication services in Startup.ConfigureServices).

after calling the `services.AddAuthentication()` method we can add  scheme-specific extension method (extensions of `IAuthenticationBuilder`), such as `AddJwtBearer` or `AddCookie`.
These extension methods use `AuthenticationBuilder.AddScheme` to register schemes with appropriate settings.

`.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => Configuration.Bind("JwtSettings", options))`

so, .AddJwtBearer is the extension method that register the scheme,
`JwtBearerDefaults.AuthenticationScheme` is the name of the scheme,
`options` is the anonymous delegate for configuration.
The actual handler is deep inside some nuget package or dll.

So the scheme is basically the index of an array of handler + configurations. The scheme is a mechanism or standard for describing challenge and forbid behaviour.

The actual middleware is registered in `Configure`.

[https://dotnetcorecentral.com/blog/authentication-handler-in-asp-net-core/](Has example of creating your own handler for the Basic auth scheme.) eg:

```c#
services.AddAuthentication("Basic")
    .AddScheme<BasicAuthenticationOptions, CustomAuthenticationHandler>("Basic", null);
```

[https://dotnetcorecentral.com/blog/asp-net-core-authorization/] custom AuthN policy handlers that implement logic, eg: User is more than twenty years old.

THe handler is a type that implements the behaviour of a scheme (as defined by the fundamental type of the scheme and the configuration options) so that is can authenticate users. Will usually construct an `AuthenticationTicket` representing the user's identity.

An authN scheme's __authenticate action__ returns an `AuthenticateResult` (success or not) and a the ticket. See AuthenticateAsync. Authenticate examples include:

+ A cookie authentication scheme constructing the user's identity from cookies.
+ A JWT bearer scheme deserializing and validating a JWT bearer token to construct the user's identity.

An authN scheme's __challenge action__ invoked on request of a secure resource. authN invokens the challenge as per the configured scheme (else the defaults). Authentication challenge examples include:

+ A cookie authentication scheme redirecting the user to a login page.
+ A JWT bearer scheme returning a 401 result with a www-authenticate: bearer header.

A __forbid action__ can let the user know:

+ They are authenticated.
+ They aren't permitted to access the requested resource.

_for multi-tenant authentication look at Orchard Core_



### Identity with/out Asp.Net Identity

Asp Identity is not any kind of requirement. The AspNetUsers table is the central table where the primary user information is stored. Linked to that is the AspNetUserLogins table which allows ASP.NET Identity to store any number of logins associated to that user. An external login contains fields for the LoginProvider (eg: "github") and ProviderKey (eg: the id is the user with google). After a (github) login the `ClaimsIdentity` get returned. This contains the claims, including the unique ID of the user on GitHub.

The ASP.NET MVC template has code which then extracts that ID, checks whether the user is signed in already and associate and external login with the user, or alternatively it will create a new user and also adds an external login for that user with the LoginProvider set to “GitHub” and the ProviderKey set to the ID of the user on GitHub. This part uses the ASP.NET Identity APIs. It's how the ASP Identity tables get populated.

ONGOING..

### WTF is Microsoft Identity Platform

I think it's the Azure version of Identity Server? it's an Identity Provider.

### Identity cookie

Needs to specify a name for the authentication scheme, which serves as the lookup string for the authentication methods configured. Default for cookie scheme is 'Cookies'

`services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => o.LoginPath = "account/hello")`

(Can also wire up events `AddCookie(o => o.Events = new CookieAuthenticationEvents(...)`) such as OnValidatePrincipal.

To authenticate a user, Core has to know the default scheme to use for those checks. This can be specified as the first param.

`CookieAuthenticationDefaults.AuthenticationScheme` contains the static value "Cookies"

In the `Configure` method we add the middleware that uses the config. Place between `.UseRouting` and before `.UseEndpoints`. `app.UseAuthentication()`. `app.UseAuthorization` is already in the default template.

`[Authorize]` checks default scheme. Can specify alt scheme as parameter.

Can add Authorize filter globally in ConfigureServices:

`services.AddControllerWithViews(o => o.Filters.Add(new AuthorizeFilter()))`

or for razor pages

`services.AddRazorPages().AddMvcOptions(o => o.Filters.Add(new AuthorizeFilter()))`

and opt out with `[AllowAnonymous]`.

see ClaimTypes enum foe built in (like NameIdentifier (unique id)). add custom types as `new Claim("foo", user.bar)`

ClaimsPrincipal - represents the user.
Will have a ClaimsIdentity for each identity scheme(default, google, facebook etc) configured.
The ClaimsIdenity object contains the Claims.
In the ClaimsPrincipal class, all the claims from each scheme are combined as a Claims property.

```c#
[HttpPost]
[AllowAnonymous]
public async Task<IActionResult> Login(LoginModel model)
{
    var user = userRepository.GetByUsernameAndPassword(model.Username, model.Password);
    if (user == null)  return Unauthorized();

    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.Name),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("FavoriteColor", user.FavoriteColor)
    };

    var identity = new ClaimsIdentity(claims,
        CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await HttpContext.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        principal,
        new AuthenticationProperties { IsPersistent = model.RememberLogin });

    return LocalRedirect(model.ReturnUrl); //limits redirect to this application
}
```

NameIdentifier is also know as SubjectId in standard claims lingo.

`SignInAsync` writes the cookie into the context.

The Identity Cookie (.AspNetCoreCookies) is created with symmetric encryption (ASP.NET Core Data Protection), key is on server. When recieved, is decrypted and used to reconstruct the ClaimsPrincipal on each request. Secured by ASP.NET Core Data Protection (which manages the key for us).

`User` is the reconstructed Claims Principal. `User.Identity.IsAuthenticated` etc...

Can iterate over `User.Claims` but there is a better way: ?WHICH IS?

## External Identity Providers

when browser requests something that requires authentication on the server, instead of an in-app login page, redirect to external uri with application's clientid and secret.
External IP returns a token (to the redirect uri's we specified during the IP setup process) with the user's claims, which can then be use to create the ASP.NET identity cookie.

add nuget (eg: MS.ANC.Authentication.Google) and

add the 'google' scheme:

```c#
.AddGoogle(o => {
  o.ClientId = Configuration["Google:ClientId];
  o.ClientSecret = Configuration["Google:ClientSecret];
})
```

Right click project > Manage User Secrets for secrets.json (don't check this into sc).
Can also do this from CLI

```bash
dotnet user-secrets init
dotnet user-secrets set "key" "val"
```

The secrets get automatically addeed to Configuration object, so code remains as normal.

### Scheme Actions (are done on the default scheme)

Authenticate: how ClaimsPrinciple gets reconstructed on every request.

Challenge: what happens when user tries to access a resource (eg: cookie scheme will redrect to the login page)

Forbid: what happend if user does not have rights. (eg: cookie scheme will redirect to Accounts/AccessDenied)

... Lets change the default Challenge to use Google instead of in-app login page:

```c#
services.AddAuthentication({o =>
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme
})
```

App will now redirect to Google login page on challenge.

## Implementing Authentication with ASP.NET Core Identity

Individual User Accounts.

Cookie based. Identity is a framework based around cookie authentication.

`ApplicationDbContext` derives from `IdentityDbContext` or `IdentityDbContext<MyApplicationUser>`. `MyApplicationUser` dervives from `IdentityUser`.

Also in startup: `services.AddDefaultIdentity<MyApplicationUser>` if using a custom type.

`[PersonalData]` attribute for account deletion and exposure.

Looked at adding the identity razor pages in (via Scaffold) and SignInManager, UserManager.

Looked at extending default User with additional proerties via new `ApplicationUser` class with new fields and EF migration.

[https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity-custom-storage-providers?view=aspnetcore-3.1](Customer Storage)

### Demo retrofit Identity (to course ConfArch app)

Adds new `ApplicationUser` and adds new scaffold via UI.

### Claims Transformation

in demo we want to see our custom claims. Makes a new `ApplicationUserClaimsPrincipalFactory` that inherits from `UserClaimsPrincipalFactory<ApplicationUser>` and has userManager and options injected in ctor

and override GenerateClaimsAsync. Finally register the new class in DI `services.AddScoped<IUserClaimsFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>()>`

also see `IClaimsTransformation`.

### Roles

Roles is off by default. Claims is default. Tur

Roles (eg: Speaker) have RoleClaims (eg: AddConfTalk).

As opposed to the User (bob) and it's UserClaims (BirthDate, FavourteCheese).

`.services.AddIdentity<ApplicationUser, IdentityRole>` (as opposed to AddDefaultIdentity). See section 3 (enabling roles) for code examples in files like ApplicationUserClaimsPrincipalFactory.

### Email functionality

implement IEmailSender, inject IConfiguration in ctor and implement SendEmailAsync.

## OICD

Centralized/Federated authentication with Identity Providers.

Identity Provider consent screen. IP provides client with tokens which contain the identity of the user / or provide access to APIs.

One IP for multiple clients. Allows single sign-on for the user across n applications.

Identity Token: personal data of user (the claims)

Access Token: When client makes a request to the API, sends access token (eg: bearer of token has access to x). Contains one or more claims. Can use same AT for multiple applications/scopes.(A scope can represent a collection of claims, or just access to one api).

How the client gets the access token is called the _flows_.

 User needs authenticating. (web app) sends client id and secret (or certificate) to Identity Providers authorization endpoint, along with required Scopes and Response type (usually tokens) and Redirect URI

IP checks everything and shows login/claims screens

after which the artefacts requested in the responseType (usually tokens, but depends on the flow / IP configuration) are delivered to the redirect uri.

see module 4.4 ~3min for example of code details in ConfArch, but conceptually:

we keep cookies with `.AddCookie()` and setup `.AddAuthentication` to use cookies for the default scheme but `OpenIdConnectDefaults.AuthenticationScheme` for the `.DefaultChallengeScheme` (so it goes to the configured IP on challenge).

`.AddOpenIdConnect` is the MS middlware that can be used with any IP that adhers to the OICD standards. Here we use an options object to config it. of note: 2 scopes 'confarch' and 'confarch_api', `.SaveTokens = true` to save the token into the httpContext. Manually mappings via `.MapUniqueJsonKey` , `.RepsponseType = "code"` = __authorization code flow__.

`options.ResponseMode = "form_post"` send client id etc in form body, not QS

front channel (browser) is considered unsafe. authorization code flow uses back channels (server to server). In this code flow, tokens (with sensitive info) are not sent to the browser, instead the browser gets a code from the IP. The code is then used by the client (the mvc web app) to do a back-channel request to token endpoint (using clientid+secret), which returns an access token to the client web app without involving the exposed browser/spa.

If OpenId scope is in the list of requested scopes, the token endpoint also send the identity token in the response.

authoriation code flow with PKCE is recommended for all interactive clients (full SPA's (public client, no secret) or those with a server based backend(confidential client)). `options.UsePkce = true`.

mobile = public channel. use back channels

Implicit flow and hybrid flow are also designed for interactive clients (involves a user), but these are no longer recommended.

Use __Client Credentials flow__ for non-interactive, confidential server to server. Client can send creds to token endpoint and get the single access token it needs (rather than all the token swapping a public client uses and all the various claims each user can have)

### 4.6 Exploring Identity Server

In the ConfArch demo, all the ASP.NET Identity stuff gets moved to ConfArch.IdentityProvider project. Of note, `ConfigureServices` gains `services.AddIdentityServer(...)` IS4 can turn any ASP.NET Core application into an OpenID Connect identity provider (with enpoints and flows).
In the options, `.AddAspNetIdentity<MyAppUserClass>()` integrates ASP Identity with IS4.

...Nuget various IS4 packages eg IdentityServer4.AspNetIdentity, MS.ANC.Authentication.JwtBearer (used by api), .IdentityModel (for console client)

__sanity check:__
browser request secure resource so login is required,
client application sends browser a redirect response,
browser follows redirect to IP, user logs in to IP,
after login the _identity (framework) cookie_ is set by Identity Framework
and returned in response to browser from IP.
_if the browser contacts the IP again, the user won't need to login as the browser will send the cookie in the request, which the IP will use to reconstruct the ClaimsPrincipal)._

The IP takes the info in the cookie (either constructed from login or passed in request) and constructs an identity token for the client app together with an access token, which is created separately.
These tokens are sent to the client using the redirect uri's we specified during the IP setup process.
When the client receives the tokens it verifies them using the crypto characteristics on the tokens. The access token is kept for later use.
The identity token from the IP is used to construct the (clients) claims principle by the client, which sets the (clients) _identity cookie_ (???is this a ASP.NET framework identity cookie format or something else???) as normal in the response to the browser.
This cookie is used from now on and the IP is no longer needed.

Browser goes to a different client app that uses the same IP. That client redirects to the IP,
but browser already has an _identity cookie_ (from the IP) so the IP skips login, reconstructs the ClaimsPrincipal and provides the tokens to thid other client via it's specified redirect url's.

IS4 uses `appsettings.config` for config. Of Note:

ApiResources lists the different APIs the IP handles auth for.
Each API has one or more scopes that are requested by the client.

With an IdentityResource the scope the client can request is the name of the resource,
eg: profile. The client then gets all claims of that named scope.

Api resources are defined per API, where an API has one or more scopes that can be requested by the client.

The various Clients are defined appsettings too: ClientId, ClientSecrets, RedirectUris etc.

## 4.9 securing the api with an access token

The API (ConfArch.API project) is set up in Startup with the AddAuthentication + UseAuthentication combo already discussed. Plus .AddJwtBearer(...) which adds the scheme that can check the token the API will be passed by the api client.

```c#
services.AddAuthentication(
    JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5000"; // is the IP this API has to trust.
        options.Audience = "confarch_api"; // special claim in the token.
        // Each API the access token is for has it's own Audience claim. Is this for me?
    });
```

The API does not need a cookie authentication scheme, just a scheme that can verify an access token.

In example the api client app (ConfArch.Web) has a ConfArchApiService which has a http client. it attaches the JWT to each request made to the api via the way the service is configured in startup.

```c#
services.AddHttpContextAccessor();
services.AddHttpClient<IConfArchApiService, ConfArchApiService>(
    async (services, client) =>
{
    var accessor = services.GetRequiredService<IHttpContextAccessor>();
    var accessToken = await accessor.HttpContext.GetTokenAsync("access_token");
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", accessToken);
    client.BaseAddress = new Uri("https://localhost:5002"); // the api to call
}
```

?I guess the context is getting the access token out from a cookie sent by the browser?

### In the next demo

they explore what happens if an external client (no cookie, maybe a different company's app) wants to call the API. First, the IP config (appsettings) is updated to define the new client ('attendeposter') using server to server (ie: client_credentials) flow.

```json
{
    "ClientId": "attendeeposter",
    "ClientName": "Attendee poster",
    "ClientSecrets": [ { "Value": "fU7fRb+g6YdlniuSqviOLWNkda1M/MuPtH6zNI9inF8=" } ],
    "AllowedGrantTypes": [ "client_credentials" ],
    "AllowedScopes": [ "confarch_api" ]
}
```

The client simply uses id and secret

```c#
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://localhost:5000");
var response = await client.RequestClientCredentialsTokenAsync(
    new ClientCredentialsTokenRequest
    {
        Address = disco.TokenEndpoint,
        ClientId = "attendeeposter",
        ClientSecret = "511536EF-F270-4058-80CA-1C89C192F69A",
        Scope = "confarch_api"
    });

```

It's a .NET client and leverages the nuget IdentityModel package.

### Access tokens (JWT)

JWT are base64 encoded.

website [https://jwt.io] to read access tokens. Can see in dev by enabling trace in logging

IP creates a hash of the token content (using either tempkey.rsa or a real key)
and encrypts that with a private key (so hash can be decrypted with a public key).
It attached the result (==signature) to the token.

Client has the public key for the IP, so can decrypt and check the hash and that nothing has been tempered with. = token verification (if the hash can be decrypted with the IP's public key, the hash must be correct). If the client then hashes the readble contents (with the public key I think) and it's the same as the decrypted hash, no readable content has been changed.

Discovery document exposes the public key so the client can decrypt

IS4 makes its private key in startup via `.AddDeveloperSigningCredental()` which generates tempkey.rsa (for dev). In production use `.AddSigningCredential()` the common overload is the one that uses an X509 cert (same as tls) see
[https://4sh.nl/IdentityServerCert] for details.

### Refresh and Reference tokens

Default is 1 hour lifetime for access token.

Refresh can be sent with access to get  a new access. To enable refresh tokens in a client, set `"AllowOfflineAccess" : true` in the IP Clients configuration bit (in appsettings.json)

The client in it's `.AddOpenIdConnet(options)` adds `options.Scope.Add("offline_access");`

An alternative to refresh tokens is reference tokens. These don't contain claims - the claims are kept on the IP. The client (mobile) uses the introspections endpoint on the IP. This is simpler but results in a lot more requests to the IP. JWT Bearer from MS does not support this, but IS4 provides something.

### IPs in the Clound

Our IP can be a client of another IP. ie: client calls IS4. IS4 calls google.

AAD (Azure AD), Auth0, Okta

## 5 Applying Authorization (done in the client app, not the IP (which does authentication))

authorization via:

+ Claim based
+ Role based
+ Resource based
+ View based

The ConfArch.Web application is using ASP.NET core authentication including claims.
These claims can be used to do authorization.

In the demo the pplication has two types of users: Organizers and Speakers.
In the (ASP.NET) Identity Framework we configured roles for these.

Policy based authorization in ConfArch.Web. Requires `app.UseAuthorization();` in Configure()
to add the middleware. Add `services.AddAuthorization(...)` in ConfigureServices.

```c#
services.AddAuthorization(options =>
{
    options.AddPolicy("IsSpeaker", policy => policy.RequireRole("Speaker"));
    // policy named "IsSpeaker" requires the claim type role with a value of "Speaker".
    // Think create policy from role

    options.AddPolicy("CanAddConference", policy => policy.RequireClaim("Permission", "AddConference")); // create policy from claim

    options.AddPolicy("YearsOfExperience",
        policy => policy.AddRequirements(
            new YearsOfExperienceRequirement(30)
            )
        );

    options.AddPolicy("CanEditProposal", policy => policy.AddRequirements(new ProposalRequirement()));
});
```

I think the `policy.RequireRole` parts needs `.AddOpenIdConnect(...)` options for
`options.TokenValidationParameters.RoleClaimType = "Role";`
to make sure it maps to the claim type "Role" we defined, we have to tell the scheme to use that instead.

ie this `options.ClaimActions.MapUniqueJsonKey("Role", "role");`

(got a bit lost here. I think this is about configuring roles, wheras claims are default)

We can create a policy for every claim we want in this way by using RequireClaim instead of RequireRole.
`.AddOpenIdConnect(...)` has `options.ClaimActions.MapUniqueJsonKey("Permission", "Permission");` so we can CREATE a policy using 
`options.AddPolicy("CanAddConference", policy => policy.RequireClaim("Permission", "AddConference"));`

and use them as attributes `[Authorize(Policy = "IsSpeaker")]`. Overrides.

In a view: (the IAuthorizationService is created when we use `app.UseAuthorization();` middleware)

```c#
@using Microsoft.AspNetCore.Authorization
@inject IAuthorizationService AuthorizationService
...
var isSpeakerAuthorizationResult = await AuthorizationService.AuthorizeAsync(User, "IsSpeaker");
```

Complex policies can be achieved using Requirements and Handlers. `IAuthorizationRequirement` marker interface and `AuthorizationHandler<T>`. 

See `YearsOfExperienceRequirement` class. Defined the Requirement that must be satisfied in order to allow access. I think this is for more complex, multiline rules that would clog up Startup if defined there.

A Resource-based policy is one that examines an object, eg: `MyPersons.CanBeEdited`.
See `ProposalApprovedAuthorizationHandler` which checks `resource.Approved`

In an API project (ConfArch.Api) can use policies to secure controller actions.
In credentials flow there is no user, so the simple option is access to evrything - but we can add an extra scope for API.

In the IP config add an extra scope `confarch_api_postattendee`

```json
Scopes": [
    {
    "Name": "confarch_api",
    "DisplayName": "ConfArch API general access"
    },
    {
    "Name": "confarch_api_postattendee",
    "DisplayName": "ConfArch API post attendee access"
    }
],
```

then in the Startup of ConfArch.Api we can add a new policy for the thing we want to secure using the scope (just the same as we did for .Web)

```c#
services.AddAuthorization(o => o.AddPolicy("PostAttendee", p => p.RequireClaim("scope", "confarch_api_postattendee"))); //scopes in the access token get added to the list of claims
```

Finally, IP client config (appsettings) needs the AllowedSCopes updates, and the api client must request the scope, in addition to the one it already had in `new ClientCredentialsTokenRequest`
eg: `Scope = "confarch_api confarch_api_postattendee` - external api-client app has 2 scopes requested.
...

### final notes

Identity token is about user's identity - not authorization data.
bloated identity tokens can cause problems (url qs limits etc).
Ideally, no claims specific to just one client in an identity token - only claims generic to all clients. Instead create a separate Authorization API where the specific client gets that data from. eg: The client or API authenticates (if a client) with the token service as normal and requests the scope for the authorization api resource (Scope: AuthService).
The access token comes back with the requested scope.
The client gets the authorizations from authorization service, sending the access token,
but also a client name so that the authorization service knows the security context requested. The Auth Service send back the authorizations.

In the client we now have policy using the requirement/handler combo which now not only inspects the claims in the identity token, but also the data fetched from the authorisaation service/api - which can then be cached.

Summary: Setting this up is hard and will take time!
