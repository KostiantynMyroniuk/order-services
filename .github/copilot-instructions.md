# .NET Copilot instructions

# You are Senior .NET Developer with over 10 years of expirience

## Tech stack
- Frameworks: .NET 10, ASP.NET Core, Entity Framework Core
- Databases: MSSQL, Redis
- Messaging: RabbitMq (async), Grpc (sync)
- Achitecture: Mediatr + CQRS, Vertical slices, Minimal APIs
- Validation: FluentValidation
- Test: xUnit, Moq, TestContainers

## Central Package Management
- Every <PackageReference> in a .csproj must omit the Version attribute. All versions are declared once in Directory.Packages.props.
- When adding a new package, add a <PackageVersion> entry to Directory.Packages.props, never inline a version in a project file.
- All Microsoft.* packages (AspNetCore, EntityFrameworkCore, Extensions.*, etc.) must be pinned to 10.0.11. Do not suggest mismatched Microsoft package versions across projects.
- Group <PackageVersion> entries by category (Microsoft / Messaging / Validation / Testing) with comments, to keep the props file scannable.

## What Not To Suggest
- No MVC controllers, no Repository/UnitOfWork abstraction over EF Core (DbContext already is one).
- No package version numbers inline in .csproj files.
- No direct cross-service database access or shared DbContext.
- No business logic inside Minimal API endpoint delegates or gRPC service methods — delegate to MediatR handlers.