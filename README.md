# Order Services

Microservices-based ordering system (e-shop) demo built on .NET.

## Architecture

- **Identity.API** issues JWT access/refresh tokens (register/login/refresh).
- **Catalog.API** owns the product catalog (SQL Server) and exposes it **only via gRPC** yet.
- **Basket.API** stores the user's cart in Redis; calls Catalog.API over gRPC to fetch product snapshots.
- **Orders.API** validates items via Catalog.API (gRPC) and persists orders (SQL Server) idempotently, using an `X-Request-Id` header + unique DB constraint.
- **Notification.Worker** — background service for notifications (scaffold, not yet implemented).

Each service owns its own database (database-per-service).

## Tech Stack

- **.NET 10** / ASP.NET Core Minimal APIs
- **MediatR** — CQRS-style request handling
- **FluentValidation** — request validation
- **Entity Framework Core** (SQL Server) — persistence
- **ASP.NET Core Identity + JWT Bearer** — auth
- **gRPC** — inter-service communication (Catalog ⟷ Basket/Orders)
- **Redis** — basket storage
- **RabbitMQ** — async messaging / event bus (planned, for Notification.Worker)
- **Docker / Docker Compose** — containerization
- **xUnit + Moq** — unit testing
