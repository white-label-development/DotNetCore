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

#### Applying Identity


Default target for `[Authorize]` is /Account/Login. Can be configured in Startup `services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Users/Login"); `

##### Roles
Same as ever ` [Authorize(Roles = "Users")]`

Default route for an action which can't be accessed is a redirect to  /Account/AccessDenied 









