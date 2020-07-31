# Authentication and Authorization in .NET Core

## Understanding

### Identity cookie
Needs to specify a name for the authentication scheme, which serves as the lookup string for the authentication methods configured. Default for cookie scheme is 'Cookies'

`services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(o => o.LoginPath = "account/hello")`

(Can also wire up events` => o.Events = new CookieAuthenticationEvents(...)`)


To authenticate a user, Core has to know the default scheme to use for those checks. This can be specified as the first param.

`CookieAuthenticationDefaults.AuthenticationScheme` contains the static value "Cookies"

In the `Configure` method we add the middleware that uses the config. Place between `.UseRouting` and before `.UseEndpoints`: `app.UseAuthentication()`. `app.UseAuthorization` is already in the default template.

`[Authorize]` checks default scheme. Can specify alt scheme as parameter.

Can add Authorize filter globally in ConfigureServices:

`services.AddControllerWithViews(o => o.Filters.Add(new AuthorizeFilter()))`

or for razor pages

`services.AddRazorPages().AddMvcOptions(o => o.Filters.Add(new AuthorizeFilter()))`

and opt out with `[AllowAnonymous]`.


see ClaimTypes enum foe built in (like NameIdentifier (unique id)). add custom types as `new Claim("foo", user.bar)`


CLaimsPrincipal - represents the user. Will have a ClaimsIdentity for each identity scheme configured. The ClaimsIdenity object contains the Claims.

The Identity Cookie is created with symmetric encryption (ASP.NET Core Data Protection), key is on server. When recieved, is decrypted and used to reconstruct the ClaimsPrincipal on each request.

## External identity providers

redirect uri. clientid and secret etc.
add nuget (eg: MS.ANC.Authentication.Google) and

add the google scheme:

```
.AddGoogle(o => {
  o.ClientId = Configuration["Google:ClientId];
  o.ClientSecret = Configuration["Google:ClientSecret];
})
```

Right click project > Manage User Secrets for secrets.json (don't check this into sc)

also from CLI

```
dotnet user-secrets init
dotnet user-secrets set "key" "val"
```

The secrets get automatically addeed to Configuration object, so code remains as normal.

Scheme Actions (are done on the default scheme)

Authenticate: how ClaimsPrinciple gets reconstructed on every request.

Challenge: what happens when user tries to access a resource (eg: cookie scheme will redrect to the login page)

Forbid: what happend if user does not have rights.

Lets change the default Challenge to use Google:

```
services.AddAuthentication({o =>
    o.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    o.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme
})
```

App will now redirect to Google login page on challenge.



## Implementing Authentication with ASP.NET Core Identity

Cookie based.

ApplicationDbContext derives from IdentityDbContext.

Looked at adding the identity razor pages in (via Scaffold) and SignInManager, UserManager.

Looked at extending default User with additional proerties via new `ApplicationUser` class with new fields and EF migration.

### Claims Transformation:
in demo we want to see our custom claims. Makes a new `ApplicationUserClaimsPrincipalFactory` that inherits from `UserClaimsPrincipalFactory<ApplicationUser>`

and override GeberateClaimsAsync. Finally register the new class in DI `services.AddScoped<IUserClaimsFactory<ApplicationUser>, ApplicationUserClaimsPrincipalFactory>()>`

also see `IClaimsTransformation`.

### Roles

Default is off. Claims is default. Tur

Roles have RoleClaims.

### Email functionality

implement IEmailSender, inject IConfiguration in ctor and implement SendEmailAsync.

## OICD

Identity Provider consent screen

Identity Token: personal data of user (claims)

Access Token: When client makes a request to the API, sends access token (eg: bearer of token has access to x). Contains one or more claims.

How the client gets the access token is called the flows.

...

Client sends client id and secret to Identity Providers authorization endpoint, along with required Scopes and Response type (usually tokens) and Redirect URI

IP checks everything and shows login/claims screens

after which the artefacts requested in the responseType are delivered to the redirect uri

...

`options.ResponseMode = "form_post"` send client id etc in form body, not QS

front channel is considered unsafe. code flow uses back channels (server to server)

authoriation code flow with PKCE is recommended for all interactive clients. `options.UsePkce = true`

mobile - public channel. use back channels

Use Client Credentials flow for non-interactive server to server. Client can send creds to token endpoint and get the single access token it needs (rather than all the token swapping a public client uses)

...Nuget various IS4 packages eg IdentityServer4.AspNetIdentity, MS.ANC.Authentication.JwtBearer (used by api), .IdentityModel (for console client)

in example (module 4.9) the client app has a ConfArchApiService which has a http client. it attaches the JWT to each request made to the api via the way the service is configured in startup.

website jwt.io ot read access tokens

can see in dev by enabling trace in logging

IP creates a hash of the token content
and encrypts that with a priate key.
It attached the resuly (==signature) to the token.

Client has the public key for the IP, so can check the hash and that nothing has been tempered with.

Discoery document exposes the public key.
IS4 makes its private key in startup via `.AddDeveloperSigningCredental()` which generates tempkey.rsa (for dev). In production use `.AddSigningCredential()` the common overload is the one that uses an X509 cert (same as tls) see
https://4sh.nl/IdentityServerCert for details.

### Refresh and Reference tokens

Default is 1 hour lifetime for access token.

Refresh can be sent with access to get  a new access. To enable refresh tokens in a client, set `"AllowOfflineAccess" : true` in the IP Clients configuration bit (in appsettings.json)

The client in it's `.AddOpenIdConnet(options)` adds `options.Scope.Add("offline_access");`


An alternative to refresh tokens is reference tokens. These don't contain claims - the claims are kept on the IP. The client (mobile) uses the introspections endpoint on the IP. This is simpler but results in a lot more requests to the IP. JWT Bearer from MS does not support this, but IS4 provides something.

### IPs in the Clound

Our IP can be a client of another IP.

AAD (Azure AD), Auth0, Okta


## 5 Applying Authorization

Policies `[Authorize(Policy = "IsSpeaker")]`

Complex policies can be achieved using Requirements and Handlers. `IAuthorizationRequirement` marker interface and `AuthorizationHandler<T>`

...

Identity toek is about user's identity - not authorization data. bloated identity tokens can cause problems (url qs limits etc)


Summary: Setting this up is hard and will take time!
