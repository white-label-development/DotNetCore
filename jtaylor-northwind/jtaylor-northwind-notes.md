# Jason Taylor's Northwind Traders

<https://github.com/jasontaylordev/NorthwindTraders>

## setup

as pe docs but changed global.json sdk version up to 3.1.401 (ther version on mu machine)

## notes from the slides, supplemented with my own observations

+ Presentation, Persistence, Infrastructure
+ Application: business-logic and types
+ Domain: enterprise-wide logic and types

Infrastructure (including Persistence) contains all external concerns

Presentation and Infrastructure depend only on Application.

### Domain

+ Avoid data annotations.
+ Use value objects wherever possible.
+ Initialise all collections and use private setters
+ Create custom domain exceptions.

Entities

Value Objects

Enumerations

Logic

Exceptions

### Application

+ Independent of infrastructure and data access concerns
Interfaces

Models

Logic

Commands /Queries: with MediatR + CQRS the application layes is just a series of request/response objects

Validators: Fluent Validation

Exceptions

### Persistence

+ Independent of the actual database (EF acts as a layer of abstration here)
+ Use Fluent API Configuration (OnModelCreating ModelBuilder stuff) over Data Anotations. [link](https://www.entityframeworktutorial.net/efcore/fluent-api-in-entity-framework-core.aspx)
+ Convention over Configuration
+ Automatically apply all entity type configurations = __WHAT?__

Uow + Repo? With EF, DbContext already acts as a unit of work; DbSet acts as a repostory.

### infrastructure

No layers depend on Infrastructyre layer.

Implementations, eg:

+ API Clients
+ File System
+ Email / SMS
+ System Clock
+ Anything external

Contains classes for accessing external resources, such as file systems, web servies etc

Implements the abstractions / interfaces that were defined within the Application layer
