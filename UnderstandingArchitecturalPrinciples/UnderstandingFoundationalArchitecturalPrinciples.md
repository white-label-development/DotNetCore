# Understanding Foundational Architectural Principles

Seperation of Concerns, DI, SR, DRY, Persistence Ignorence etc..

## Clean Architecture

DI provides loose coupling between layers.

UI/Infrasructure -> Application Core > Entities/Interfaces

### Core

+ Entities
+ Interfaces (of Core and Infrastructure)
+ Services
+ Exceptions

No deps on Infrastructure (no EF or serilog etc)

### Infrastructure

+ Data Access (EF)
+ Logging
+ Identity
+ API clients
+ File Accesss

In demo

/Core

+ .Application
+ . Domain

/Infrastructure

+ .Identity
+ .Infrastructure
+ Persistence

/UI

+ App

/API

+ Api

## 4 Setup Application Core

blah...

.Application/Contracts/Persistence

contains interfaces (the contracts I guess), ef: IAsyncRepository

@4 cqrs