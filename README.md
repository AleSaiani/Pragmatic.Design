# Back-end

### Requirements

.NET 7.0.402 (NB: previous version of .NET 7 has some bug in sourge generation)

#### Installation Steps

``` 
dotnet tool install csharpier -g 
```

```
npm install
```

## Architecture

The primary responsibility of the backend is to manage the application domain's modeling, handle its persistence, and expose REST APIs. We are considering "CRUD" projects, where the backend generally provides APIs to a Single-Page Web App (SPA) and manages persistence using EF Core with a relational database (e.g., Postgres, SQL Server). The architecture employed is based on simplifying a CQRS/DDD approach in a "basic" version of the model based on REPR (Request-Endpoint-Response) following the "minimal API" approach.

We'll carry over the concept of DDD entities but simplify the CQRS pattern into a streamlined version based on the concept of Domain Actions alone. A Domain Action is the element through which any interaction (both read and write) with the application domain occurs. Invoking one or more Domain Actions can generate Domain Events. These are recommended tools for developing the required application features. Of course, if this approach isn't suitable in certain scenarios, features can be developed in a more "classic" manner.

Domain modeling involves a series of Entities or Value Objects that don't directly contain any persistence-related information. Relationships between these entities are represented by EF Core's Navigation Properties. Unless rare cases, it's advisable to follow a unidirectional relationship modeling, from parent entity to child entity. This helps avoid potential cyclic navigations between entities and accurately represents the "responsibilities" among entities. An exception to this rule is, of course, self-referencing entity relationships (hierarchies).

The architecture consists of a very simple stratification:

-   A Host layer serving as the backend's entry point.
-   One or more applications "exposed" by the host, represented by a class library.
-   One or more "non-domain" features, represented by a class library or a NuGet package.

### Host

This is a typical AspNet Core WebAPI application. The host's responsibility is to start up the application. The approach used is "configuration" based. Enabling/disabling or configuring low-level functionalities should be achievable through configuration rather than code modification. The host includes mechanisms for autodiscovery of:

-   Endpoints
-   Entities composing the ApplicationDBContext
-   Services to register in DI (Dependency Injection)
-   Mapping
-   Validations
-   and more

In the host layer, a generic DbContext (Pragmatic.Design.Core.Persistence.ApplicationDBContext) can be used to register (manually or through autodiscovery) domain entities. This layer is responsible for generating migrations. It's also possible to apply migrations during startup if configured. The startup process manages initial exceptions, which, if they occur, put the application in a "maintenance" state, displaying a user-friendly message and allowing developers quick access to log information to identify issues. The host relies on the FastEndpoint and Mediator libraries.

### Application

This is a class library where all aspects of an application are placed, including:

-   Entities
-   Domain Actions (business logic methods)
-   Domain Events
-   Entity persistence information
-   Validations
-   DTOs (Data Transfer Objects)

These elements can all be organized into separate files. To simplify development, a more compact organization of these elements is provided based on two solutions:

-   Nested classes
-   Interfaces with static methods

The aim of this simplification is to keep various configuration aspects "close" to the element being configured. For example, with the IHasPersistence interface, a class can be decorated to be registered by the ApplicationDbContext, explicitly stating its persistence configurations. The classic method based on IEntityTypeConfiguration<T> can still be used.

#### Entities

These are the classes representing domain entities or domain Value Objects. In the same file, the following aspects can be configured:

-   Validation: Create a nested class Validator extending FluentValidation's AbstractValidator<T>
-   Persistence: Implement the IHasPersistence interface

#### Domain Actions

These are Business Logic operations towards the domain. We don't maintain a formal distinction between read and write operations (which change the domain). This distinction is left to the developer. For example, a convention might be that if a Domain Action starts with "Get," it's a read operation without modifying the state. However, nothing in the system enforces this.

A Domain Action is obtained by implementing the IDomainAction<TRequest, TResponse> interface (or IDomainAction<TResponse>). This interface has a method to implement with the following signature:

`public async ValueTask<Response> Handle(Root root, CancellationToken cancellationToken);` 

A Domain Action is invoked via Mediator. One Domain Action can invoke other Domain Actions. Although this can be done using async/await, the goal is to sequence the operations, as the result of the calling Domain Action depends on the invoked operations. The Handle method serves as the Mediator's handler for the request<TRequest, TResponse>. The Root class acts as a helper class for accessing elements through Dependency Injection.

A Domain Action can be exposed via API by:

-   Implementing the IHasEndpoint interface (or IHasCustomEndpoint)
-   Decorating the Domain Action class with attributes like HttpGet/HttpPost/HttpPut, specifying the route template
-   Decorating the Domain Action with Authorize attribute
-   Defining Swagger generation information for the endpoint using [SwaggerOperation] and [SwaggerResponse]

Though these attribute names are the same, they are implementations present in the Pragmatic.Design project (keeping the original format as much as possible).

For an endpoint, you can create a nested class that derives from Validator<T> (a FastEndpoint class for validation using Fluent Validation). This automatically validates the request upon receiving it by the host, even before considering it. This class also automatically adds a status code 400 to possible results.

#### DTOs

We mostly use POCO classes as Data Transfer Objects for all Domain Actions. These classes should be placed as follows:

-   As nested classes of the Domain Action if used only for that action
-   In the first parent folder of Domain Actions using that DTO if it's reused in more than one action.

Mapping is done through AutoMapper. To configure mapping, just implement the IAutomapper interface on the Domain Action class.

#### Folder Structure

Entities and related Domain Actions can be organized and partitioned through various folder structures. You can create a folder for each entity and subfolders for different domain areas to place the Domain Actions for that entity within that area. Alternatively, you could create a folder for the domain's Boundary Context and group entities and Domain Actions inside. Each application might have different needs, although the first proposed solution could be sufficiently suitable for "standard" situations. But reorganizing this logical structure won't impact the application's functionality and is easily achievable by creating folders and moving individual files.

In general, a good rule to follow when evaluating a structure is that it should be possible to move independent portions of the domain to another application with a minimal amount of copy-pasting.

### Feature

Based on the principle described in the previous paragraph, when an independent feature is identified (a set of entities/domain actions/etc), a library can be created to contain that functionality. This can result in a NuGet package or can be quickly copy-pasted into another application following the structure described in this README.