### Identity

The user class is derived from IdentityUser.

In Startup, setup EF Core
```
services.AddDbContext<AppIdentityDbContext>(options =>
                options.UseSqlServer(Configuration["ConnectionStrings:SportStoreIdentity"]));
```

and add services

```
services.AddIdentity<AppUser, IdentityRole>(opts => {
                    opts.User.RequireUniqueEmail = true;                    
                }).AddEntityFrameworkStores<AppIdentityDbContext>()
                .AddDefaultTokenProviders();
```

The AddEntityFrameworkStores method specifies that Identity should use Entity Framework Core to store and retrieve its data, using the database context class.

AddDefaultTokenProviders method uses the default configuration to support operations that require a token, such as changing a password.

and in Configure, add `app.UseAuthentication()` 
which allows user credentials to be associated with requests based on cookies or URL rewriting.

In powershell: `dotnet ef migrations add Initial` and `dotnet ef database update`

Can make  `CustomPasswordValidator : IPasswordValidator<AppUser>` and registering  `services.AddTransient<IPasswordValidator<AppUser>, CustomPasswordValidator>();`

Can make  `CustomUserValidator : IUserValidator<AppUser>`  ...  `services.AddTransient<IUserValidator<AppUser>, CustomUserValidator>();`

#### Seed Db

Can simple seed a single admin via appsettings.json
```
AdminUser": {
    "Name": "Admin",
    "Email": "admin@example.com",
    "Password": "secret",
    "Role": "Admins"
  }
```

add static CreateAdminAccount method to AppIdentityDbContext, and call it from Startup.Configure
```
AppIdentityDbContext.CreateAdminAccount(app.ApplicationServices, Configuration).Wait(); 
```

#### Updating Identity classes (AppUser)

If we add noew fields to AppUser and need to create a new EF migration remember to disable (comment out) any seeding
as EF can't cope.  The seeding statement can be enabled again once the database migration has been created and applied.

PS: `dotnet ef migrations add CustomProperties` and `dotnet ef database update`



#### Applying Identity


Default target for `[Authorize]` is /Account/Login. Can be configured in Startup `services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Users/Login"); `

##### Roles
Same as ever ` [Authorize(Roles = "Users")]`

Default route for an action which can't be accessed is a redirect to  /Account/AccessDenied 


##### Claims and Policies

A claim is a piece of information about the user, along with some information about where the information came from.

Identity and Roles use Claims already, "LOCAL UATHORITY, Name = Neil", "LOCAL UATHORITY, Role = Foo", etc

Claims are interesting because an application can obtain claims from multiple sources, rather than just relying on a local database for information about the user.

Claims are used to build authorization policies, which are part of the application configuration and applied to action methods or controllers using the Authorize attribute.

```
services.AddAuthorization(aopts =>
    {
        aopts.AddPolicy("DCUsers", policy =>
        {
            policy.RequireRole("Users");
            policy.RequireClaim(ClaimTypes.StateOrProvince, "DC");
        });
    }); //  only allows access to Users with a specific claim type and value
```

applied as an attribute
```
[Authorize(Policy = "DCUsers")]
public IActionResult OtherAction() => ...
```

The policy system can be extended with custom requirements, which are classes that implement the IAuthorizationRequirement interface, and custom authorization handlers, which are subclasses of the AuthorizationHandler class that evaluate the requirement for a given request

eg: create new class `BlockUsersRequirement : IAuthorizationRequirement` and `BlockUsersHandler : AuthorizationHandler<BlockUsersRequirement>`
```
opts.AddPolicy("NotBob", policy => {policy.RequireAuthenticatedUser(); 
    policy.AddRequirements(new BlockUsersRequirement("Bob"));   // block "Bob" from Action with attribute [Authorize(Policy = "NotBob")]              
});
```

The claims system can be applied to resources (files etc).


#### Third Party Authentication

eg: Google https://developers.google.com/identity/  then 'Integrate Google Sign-In'

Specify a callback URL (use the webserver option), which for the default configuration is /signin-google. 
If you are in development, set the callback URL to be http://localhost:49448/signin-google. 

= we have a client ID and a client secret. 

Enable the Google+ API (forgot this the first time and got a Forbidden error)

```
services.AddAuthentication().AddGoogle(opts => { opts.ClientId = "<enter client id here>";opts.ClientSecret = "<enter client secret here>"; })
```

When you authenticate a user with a third party, you can elect to create a user in the Identity database, 
which can then be used to manage roles and claims just as for regular users.

In the User project solution, if the 3rd party auth succeds and the user does not exist locally, we manually add a new AppUser
from the Account.GoogleResponse() method.