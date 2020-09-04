# Invoice Management App exploration

 https://alexcodetuts.com/2020/02/05/asp-net-core-3-1-clean-architecture-invoice-management-app-part-1/

Domain: contains the entities or types

Application: contains business logic and depends on domain layer.

Presentation / Infrastructure: depends only in Application layer.

Infrastructure contains external concerns like the type of database while Presentation layer is responsible for presenting the data to the client application

+ Presentation (the web app) & .Infrastructure depend on > .Application
+ Presentation also depends on > Infrastructure (so `Program` can see `ApplicationDbContext`)
+ .Application depends on .Domain
+ .Domain has no project dependencies

## 1 Setup

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

## 2 EF Core Auditable Entities

Add Domain items: AuditEntity base class, Invoice, invoiceItems and Enums.

In Application

+ Install-Package Microsoft.AspNetCore.Identity.EntityFrameworkCore

and add /Common/Infrastructure/IApplicationDbContext

```c#
public interface IApplicationDbContext
{
    DbSet<Invoice> Invoices { get; set; } 
    DbSet<InvoiceItem> InvoiceItems { get; set; } 
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
```

then update `InvoiceManagementApp.Infrastructure.Data.ApplicationDbContext` to implement the interface and ctor inject `ICurrentUserService`. Add the `SaveChangesAsync` overrode to implement the audit stuff (sets properties) for any entity of `AuditEntity` type.

Update the DI class: `services.AddScoped<IApplicationDbContext>(provider => provider.GetService<ApplicationDbContext>());`

Next we need `ICurrentUserService` set up:

Update the presentation layer to have a dep on .Application. Add `/Services/CurrentUserService.cs`. This sets `UserId` from the ClaimsPrincipal in the ctor using

```c#
public CurrentUserService(IHttpContextAccessor httpContextAccessor)
{
    UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
}
```

In `StartUp` register `ICurrentUserService` in `ConfigureServices` via `services.AddScoped<ICurrentUserService, CurrentUserService>();`

Finally add a migration as we updated the DbContext. `add-migration AddInvoice` and `update-database`

## 3 MediatR and FluentValidation

In .Application add

+ FluentValidation.DependencyInjectionExtensions
+ MediatR.Extensions.Microsoft.DependencyInjection

add 

+ Invoices/Commands/CreateInvoiceCommand.cs -  `public class CreateInvoiceCommand : IRequest<int>`
+ Invoices/Handlers/CreateInvoiceCommandHandler.cs - `public class CreateInvoiceCommandHandler: IRequestHandler<CreateInvoiceCommand, int>`. Implements `Handle` method
+ Invoices/ViewModels/CreateInvoiceCommandHandler.cs - vm for Domain `InvoiceItem`

add

+ Common/Behaviours/ValidationBehaviour.cs - generic validation handler for MediatR Pipeline.
+ Invoices/Validators/CreateInvoiceCommandValidator.cs - rules for `CreateInvoiceCommand` Request object.
+ Invoices/Validators/MustHaveInvoiceItemPropertyValidator.cs - custom validator for `CreateInvoiceCommand.InvoiceItems` property.
+ add /DependencyInjection.cs

then write up Application.DependencyInjection to Startup via `services.AddApplication();`

Next add an API endpoint in InvoiceManagementApp (web/api project)

+ Controllers/ApiController.cs - the abstract base class
+ Controllers/InvoiceController.cs - POST `Create(CreateInvoiceCommand command)`

Note: Controllers/ApiController uses

```c#
protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetService<IMediator>();
// GetService uses Microsoft.Extensions.DependencyInjection. Baffled me for a moment.
```

Note "??= assigns the value of its right-hand operand to its left-hand operand only if the left-hand operand evaluates to null"

We have a POST, lets add a GET = Query

Add

+ .Application/Invoices/Queries/GetUserInvoicesQuery.cs - query param `string User`
+ .Application/ViewModels/InvoiceVm.cs = VM for response object (list of)
+ .Application/Invoices/Handlers/GetUserInvoicesQueryHandler.cs - given a `GetUserInvoicesQuery`, return a `IList<InvoiceVm>`
+ update InvoicesController with `Get()`

## 3 1/2 A segue into https://www.codewithmukesh.com/blog/cqrs-in-aspnet-core-3-1/

Add

+ .Domain/Entities/Product.cs
+ update ApplicationDbContext, Add-Migration, Update-Database
+ .Application/Products/{Commands, Queries, ViewModels}

In this version (which I prefer) the Handler is within the Command or Query, rather than it's own class in the /Handlers folder. The major advantage here is that you can "Go to Implementation" from the controller, eg:

```
public class ProductController : ApiController
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateProductCommand command)
    {
        return Ok(await Mediator.Send(command));
    }
}
```

_we can click `CreateProductCommand` to get to the next logical part._

While we are here, lets add Swagger so we need Postman less:

`Install-Package Swashbuckle.AspNetCore.Swagger` and `Install-Package Swashbuckle.AspNetCore` and update Startup.cs for basic open swagger with no xml comments scanning or client generation etc.


## 4 Automapper (inevitable)


## 5
## 6
## 7
