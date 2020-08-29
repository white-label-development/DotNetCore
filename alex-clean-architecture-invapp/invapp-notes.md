# Invoice Management App exploration

from https://alexcodetuts.com/2020/02/05/asp-net-core-3-1-clean-architecture-invoice-management-app-part-1/

Domain: contains the entities or types
Application: contains business logic and depends on domain layer.
Presentation / Infrastructure: depends only in Application layer.
Infrastructure contains external concerns like the type of database while Presentation layer is responsible for presenting the data to the client application

## 1

Created:

+ InvoiceManagementApp - ASP.NET Core Web Application w/ react
+ InvoiceManagementApp.Application - Class Lib .NET standard
+ InvoiceManagementApp.Domain - Class Lib .NET standard
+ InvoiceManagementApp.Infrastructure - Class Lib .Net Core (so IS4 can installed)

The ApplicationDbContext should be in Infrastructure layer, but currently it is in presentation layer...In infrastructure

+ Install-Package Microsoft.AspNetCore.ApiAuthorization.IdentityServer
+ Install-Package Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore
+ Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore
+ Install-Package Microsoft.EntityFrameworkCore.SqlServer
+ Install-Package Microsoft.EntityFrameworkCore.Tools

Cut and paste the Data and Models folder from InvoiceManagementApp to InvoiceManagementApp.Infrastructure. Fix `Startup.cs/ConfigureServices()` and other broken references.

Delete the Migrations folder.

Update the namespace of ApplicationDbContext and ApplicationUser to use InvoiceManagementApp.Infrastructure.

Create DependencyInjection.cs in Infrastructure root. Adds an extension method `AddInfrastructure()` to the `IServiceCollection`.

 Update `Startup.cs/ConfigureServices()` with `services.AddInfrastructure(Configuration);`

 Update `Program.cs/Main()` to run `context.Database.Migrate();`

 and in Infrastructure run `add-migration InitialMigration` and `update-database`

 Note: uses localDb, so it's just an .mdf in Users dir. Use VS > View > SQL Server Object Explorer to examine.

## 2


## 3
## 4
## 5
## 6
## 7
