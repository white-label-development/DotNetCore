# Securing ASP NET Core 3 with OIDC

IDP will authenticate the user and provide proof of identity to an application

OAuth2 is about authorization. Standard endpoints. authorization tokens.

OpenID Connect extends OAuth2 brings in identity tokens authentication.

## 3 OIDC

focus here is on Identity tokens, not access tokens.

Client App (sometimes called "relying party") requires a users identity. User is redirected to IDP , logs in there and IDP sends identity token back. Client App validates the token. In Net, the identity token is then used to create a Claims identity - usually sored as an ticket in an encrypted cookie.

**Confidential client** can maintain the confidentiality of their (clientid, client secret) secrets. eg a server app (asp mvc, SSBlazor) can safely authenticate and store.

**Public clients** vsnnot maintain confidentiality of their secrets, eg: browser apps. These client apps cannot safely authenticate.

#### Flows

How the code/tokens are returned to the client. Flows determine the endpoints.

Auth endpoint (IDP level): used by the client application to obtain authentication and/or authorization, via redirection. Rquires TLS ofc.

Redirection (callback) endpoint (client level): used by the IDP to return code and tokens to the client app. ie: the URI used in the IDP callback.

Token endpoint (IDP level): used by client to request tokens (directly, no redirection) from the IDP

##### The flows:

**Authorization Code Flow:** tokens from token endpoint. for confidential clients. long lived access (refresh tokens). Should support PKCE

**Implicit Flow:** returns all tokens from authorization endpoint. There is no authorization code. Token endpoint is not used. for public clients. no client authentication no long lived access.

**Hybrid Flow:** returns some tokens from authorization endpoint and others from the token endpoint. for confidential clients. long lived access (refresh tokens).


OIDC Flow for MVC Client (confidential). Best practice is Authorization Code Flow, but Hybrid is ok.



#### IS4 middleware

demo: install the tempates `dotnet new -i IdentityServer4.Templates`, then new Project using the empty is4 template. Of note: in properties, enable ssl and ser the Launch to "Project" - which will open a console so we can see what is going on. Finally set the App URL (in debug?) to localhost:44318, and uncheck "Launch browser"

Test it works by running the server and checking the well known endpoint.

Stage2 - lets add some UI pages for login etc to the IS4 project. Grab the Quickstart template (this is all pretty much identical to IS4 documentation) 
`dotnet new is4ui` - should add some new folders to the project (Quickstart, Views)

In startup uncomment the mvc stuff eg: `services.AddControllersWithViews`

See TestUsers.cs for Users and Claims. Claims are related to scopes -> in Config.cs

IdentityResources map to scopes that give access to identity related information.

added `new IdentityResource.Profile()` to allow access to profile related claims, like given_name. Profile is a scope I think

ApiResources map to scopes that give access to apis.

## 4 Securing your user authentication processes

**Authorization Code Flow:** authentication request goes to an endpoint. response_type determines the flow, eg: = code.

Front channel = info to browser via URI or POST (response_type=code)

Back Channel = server to server. browser does not see this.

jwt.io

UserInfo endpoint (IDP level): used by client to request additional user claims from the IDP. Requires an access token with scopes related to the claims wanted

.GetClaimsFromUserEndpoint in startup

logout from IDP as well.

IdentityTokens are JWT

**Hybrid Code Flow:** code AND id_token returned at start (after authentication and consent)

does not require PKCE and the id_token is protected by the nonce. plus other things I am skipping.


## 5 Working with Claims

Claims transformation - claims returned from an IDP are sometimes renamed to other claim types. Stop this with `DefaultInboundClaimType.Clear()`

Can remove claims from being filtered out by the middleware, and delete claims so they are never shown.

Can call UserInfo endpoint (to get additional info) manually (on a per function basis): Nuget IdentityModel ... get address claim ... See OrderFrame() in demo


### Role Based Authorization RBAC

`new Claim("role", "fooUser");`

Role scope isn't a standard scope so in the IDP
`new IdentityResorce("roles", "Your role(s)", new List<string>(){"role"});` also add "roles" to `AllowedScopes`.

In client, `options.Scope.Add("roles")` and mapping `options.ClaimActions.MapUniqueJsonKey("role", "role");`

Finally in client (ffs)
```
options.TokenValidationParameters = new TokenValidationParameters
{
    NameClaimType = JwtClaimTypes.GivenName,
    RoleClaimType = JwtClaimTypes.Role
};
```
 can then user `User.InRole` and [Authorize(Roles="fooUser")]

 #### An access denied page
```
services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.AccessDeniedPath = "/Authorization/AccessDenied";
})
            ...
```


## 6 Understanding Authorization

OAuth2 High Level: Client authenticates, requests access token and sends it as a bearer token (give access to bearer) on each request to the API. At the API the access token is validated and access to the resources can be granted.

OICD is superior and we should always use it. identity token can be linked to the access token (at_hash). verification of that = better security. the nonce stops redirect attacks



## 7 Securing your API

In IDP Config add an ApiResource. add to AllowedScoped.

In Client `options.Scope.Add("imagegalleryapi")`

In API

Nuget AccessTokenValidation package

In startup
```
services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
.AddIdentityServerAuthentication(options =>
{
    options.Authority = "https://localhost:44318";
    options.ApiName = "imagegalleryapi";
}); //AuthenticationScheme = bearer token authentication. IDP is :44318

....

app.UseAuthentication();
app.UseAuthorization();
```

in ImageController add an [Authorize] attribute to kick things off.

Lastly, *we need to pass the access token to the api:*

We could manually add the token to the httpClient request but better to encapsulate this

```
public class BearerTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public BearerTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ??
            throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken)
    {
        var accessToken = await _httpContextAccessor
            .HttpContext.GetTokenAsync(OpenIdConnectParameterNames.AccessToken);

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            request.SetBearerToken(accessToken);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
```

then in startup

```
services.AddHttpContextAccessor(); // handler needs this so we have to register it
services.AddTransient<BearerTokenHandler>();

// create an HttpClient used for accessing the API
services.AddHttpClient("APIClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:44366/");
    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
}).AddHttpMessageHandler<BearerTokenHandler>(); // <- add handler here
```

#### Using access token claims in the API

`var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;` where sub is the id `var imagesFromRepo = _galleryRepository.GetImages(ownerId);`

Rather than have this check in each function, we can block access to the action

note: The access token contains scopes (like sub) but not user claims. Sometime, an api needs access to the identity claims. The IDP userinfo endpoint is intended to be called from the client, not the api.
It's better to include the claims in the access token when asking for a resource scope by including them in the claims list related to that scope.

lets include the role claims in the access token:

in IDP config we can define
```
new ApiResource(
    "imagegalleryapi", 
    "Image Gallery API",
    new List<string>() { "role" })
```
that is all we need to do to include it. In the API ImagesController.CreateImage() we can decorate `[Authorize(Roles = "PayingUser")]`
note use of id from `var ownerId = User.Claims.FirstOrDefault(c => c.Type == "sub").Value;` rathen than an id passed in the inbound model (which can be tampered with)






## 8 Authorization Policies

superior to Roles

@skipped

## 9 Authorization Policies

@skipped

## 10 Authorization Policies

#### signing cert

IDP startup `builder.AddDeveloperSigningCredential();`. if a LB is used, will be different across instances, AppPool recycle will also lose this. Need a real key.

Can use Raw RS (SHA256) keys (no expiry) or a signing certificate (expires) stored in IDP, a windows cert store or Azure Key Vault.

Demo: self signed cert in windows (assume single instance) in powershell (admin): `New-SelfSignedCertificate -Subject "CN=MarvinIDPCert" -CertStoreLocation="cert:\LocalMachine\My"`

Should be in machine certs 'Personal' folder. Copy thumbprint to notepad for later. To make cert trusted, cpoy it and paste into "Trusted Root Cert auth" > certficate folder.

In IDP Startup add method

```
public X509Certificate2 LoadCertificateFromStore()
{
    string thumbPrint = "d4d681b3de4cd26fc030292aeea170e553810bdb"; <- paste from notepad. check for invisile character at front with a delete.

    using (var store = new X509Store(StoreName.My, StoreLocation.LocalMachine))
    {
        store.Open(OpenFlags.ReadOnly);
        var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint,
            thumbPrint, true);
        if (certCollection.Count == 0)
        {
            throw new Exception("The specified certificate wasn't found.");
        }
        return certCollection[0];
    }
}

//replace builder.AddDeveloperSigningCredential() with 
builder.AddSigningCredential(LoadCertificateFromStore());
```

jwks kid should now match thumbprint.

#### config data

rather than hardcode into config the clients, apis, reources etc they can come from a data store and be updated by a tool (sold by is4 people) (rather than need a redeploy). Need a shared persistant data store...

NuGet IdentityServer4.Entityframework package && MS.EntityFrameworkCore.SqlServer && MS.EntityFrameworkCore.Design

Add connstr (should be in a valut or something but for demo, IDP startup)
```
 public void ConfigureServices(IServiceCollection services)
        {
            var marvinIDPDataDBConnectionString = 
                "Server=(localdb)\\mssqllocaldb;Database=MarvinIDPDataDB;Trusted_Connection=True;";
...

var migrationsAssembly = typeof(Startup).GetTypeInfo().Assembly.GetName().Name; // get current assembly name (Marvin.IDP)
builder.AddConfigurationStore(options =>
{
    options.ConfigureDbContext = builder => 
        builder.UseSqlServer(marvinIDPDataDBConnectionString,
        options => options.MigrationsAssembly(migrationsAssembly)); //where to find the migrations as context is in identityserver4.entityframework - but we want the migrations in this project
});    

// remove AddInMemoryX calls.
```
then

`add-migration -name InitialISMigration -context ConfigurationDbContext` (ConfigurationDbContext comes from dentityserver4.entityframework package )

Demo: Seed data from what we already have in config file: see `private void InitializeDatabase(IApplicationBuilder app)` in demo. Then `InitializeDatabase(app);` in configure.

Operational data (i think this is keys a which were held in memory - see PersistedGrants table) stored in a similar way
```
builder.AddOperationalStore(options =>
{
    options.ConfigureDbContext = builder => 
        builder.UseSqlServer(marvinIDPDataDBConnectionString,
        options => options.MigrationsAssembly(migrationsAssembly));
});
```
`add-migration -name InitialISOperationalMigration -context PersistedGrantDbContext`

