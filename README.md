# MicroservicesHub

An e-commerce backend built with **.NET 8** following a microservices architecture. Each service is independently deployable, owns its own data store, and communicates via REST or gRPC. The codebase demonstrates **Clean Architecture**, **CQRS**, and **Domain-Driven Design** patterns using modern ASP.NET Core libraries.

---

## Architecture

```
┌─────────────────┐     REST      ┌─────────────────┐
│   Catalog.API   │               │   Basket.API    │
│  (Products)     │               │  (Shopping Cart)│
│  Marten + PG    │               │  Marten + PG    │
└─────────────────┘               └────────┬────────┘
                                           │ gRPC
                                  ┌────────▼────────┐
                                  │  Discount.Grpc  │
                                  │  (Coupons)      │
                                  │  EF Core+SQLite │
                                  └─────────────────┘

┌──────────────────────────────────────────────────┐
│               Ordering.API (WIP)                 │
│   Clean Architecture: API / Application /        │
│   Domain / Infrastructure layers                 │
└──────────────────────────────────────────────────┘
```

All REST services share a **BuildingBlocks** library providing CQRS interfaces, MediatR pipeline behaviors (validation + logging), and a global exception handler.

---

## Services

| Service | Responsibility | Database | Protocol |
|---------|---------------|----------|----------|
| **Catalog.API** | Product catalog — CRUD, pagination | PostgreSQL (Marten) | REST |
| **Basket.API** | Shopping cart — store, retrieve, delete with Redis caching and discount application | PostgreSQL (Marten) + Redis | REST |
| **Discount.Grpc** | Coupon management — get/create/update/delete | SQLite (EF Core) | gRPC |
| **Ordering.API** | Order lifecycle (in progress) | TBD | REST |

---

## Tech Stack

| Category | Technology |
|----------|-----------|
| Runtime | .NET 8 / ASP.NET Core 8 |
| API routing | [Carter](https://github.com/CarterCommunity/Carter) (minimal APIs) |
| CQRS mediator | MediatR 12 |
| Object mapping | Mapster |
| Validation | FluentValidation |
| Document store | Marten 7 (PostgreSQL) |
| Relational ORM | Entity Framework Core 9 |
| Distributed cache | Redis (StackExchange.Redis) |
| Service decoration | Scrutor |
| Inter-service RPC | gRPC (Grpc.AspNetCore) |
| Containerization | Docker + Docker Compose |

---

## Running Locally

**Prerequisites:** Docker Desktop installed and running.

```bash
# Clone and start all services + infrastructure
git clone <repo-url>
cd "MicroservicesHub - Copy/src"

docker-compose -f docker-compose.yml -f docker-compose.override.yml up --build
```

Services will be available at:

| Service | HTTP | HTTPS |
|---------|------|-------|
| Catalog.API | http://localhost:6000 | https://localhost:6060 |
| Basket.API | http://localhost:6001 | https://localhost:6061 |
| Discount.Grpc | http://localhost:6002 | https://localhost:6062 |

Infrastructure ports: PostgreSQL (Catalog) `5432`, PostgreSQL (Basket) `5433`, Redis `6379`.

Health checks are available at `GET /health` on each REST service.

### Running a single service locally

Ensure the required infrastructure (PostgreSQL / Redis) is running via Docker, then:

```bash
dotnet run --project src/Services/Catalog/Catalog.API
```

---

## Configuration

Each service reads from `appsettings.json` and `appsettings.Development.json`. Docker Compose overrides these via environment variables using the `__` double-underscore convention for nested keys.

| Key | Service | Purpose |
|-----|---------|---------|
| `ConnectionStrings__Database` | Catalog, Basket, Discount | Primary DB connection string |
| `ConnectionStrings__Redis` | Basket | Redis connection string |
| `GrpcSettings__DiscountUrl` | Basket | URL of the Discount gRPC service |

Local developer secrets are stored via `dotnet user-secrets` and mounted into containers from `%APPDATA%/Microsoft/UserSecrets`.

---

## Key Design Decisions

**CQRS + MediatR pipeline** — Commands and queries are strictly separated using shared `ICommand<T>` / `IQuery<T>` interfaces. Every request passes through `ValidationBehavior` (FluentValidation) and `LoggingBehavior` (warns if >3 s) before reaching its handler.

**Feature-folder structure** — Each operation (e.g., `CreateProduct`) lives in its own folder containing the command/query record, validator, handler, and Carter endpoint module. This keeps cohesive logic co-located.

**Mixed persistence strategy** — Catalog and Basket use Marten's document store API over PostgreSQL for schema-free flexibility. Discount uses EF Core with SQLite because coupon data is relational and rarely changes.

**Decorator pattern for caching** — `CachedBasketRepository` wraps `BasketRepository` using Scrutor's `Decorate<>`, implementing a cache-aside pattern with Redis without polluting the core repository logic.

**gRPC for internal RPC** — The Basket service calls Discount over gRPC (HTTP/2) to apply coupon prices during cart operations, keeping the discount contract typed via Protocol Buffers (`discount.proto`).

**Ordering uses Clean Architecture** — Unlike the other services, Ordering is structured into four layers (Domain → Application → Infrastructure → API) with DI composition via extension methods, serving as the DDD reference layer in the solution.

---

## Solution Structure

```
src/
├── BuildingBlocks/BuildingBlocks/   # Shared CQRS, behaviors, exceptions
├── Services/
│   ├── Catalog/Catalog.API
│   ├── Basket/Basket.API
│   ├── Discount/Discount.Grpc
│   └── Ordering/
│       ├── Ordering.API
│       ├── Ordering.Application
│       ├── Ordering.Domain
│       └── Ordering.Infrastructure
├── docker-compose.yml
├── docker-compose.override.yml
└── eshop-microservices.sln
```
