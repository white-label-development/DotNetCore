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
