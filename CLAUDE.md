# TaskProject — Claude Code Instructions

## Project Overview

Task manager application with an ASP.NET Core Web API backend and Angular frontend.

- **Solution:** `TaskProject.slnx`
- **Framework:** .NET 10
- **Domain:** CRUD-heavy task management (TaskItem entities with status transitions)

## Solution Structure

```
TaskProject/
├── src/
│   ├── TaskProject.Api/       # ASP.NET Core Web API (controllers, Program.cs)
│   └── TaskProject.Core/      # Domain library (entities, services, EF Core, migrations)
│       ├── Data/               # AppDbContext + EF migrations
│       ├── Entities/           # Domain entities (TaskItem, TaskItemStatus)
│       └── Services/           # Business logic (TaskService, ServiceResult)
├── tests/
│   └── TaskProject.Tests/     # NUnit test project
├── frontend/                   # Angular SPA
└── docker-compose.yml
```

## Architecture

**Layered (Api → Core)** — keep it simple, CRUD-heavy domain.

- `TaskProject.Api` depends on `TaskProject.Core`
- No additional layers — do not introduce Application/Infrastructure/Domain splits unless explicitly asked
- Controllers are thin; business logic lives in services
- Services return `ServiceResult<T>` — use this pattern for all service methods

### ServiceResult Pattern

```csharp
// Always return ServiceResult<T> from service methods
ServiceResult<TaskItem>.Success(item)
ServiceResult<TaskItem>.NotFound()
ServiceResult<TaskItem>.Failure("error message")

// In controllers: check IsNotFound, IsSuccess, Error, Value
```

## Tech Stack

| Concern       | Choice                                      |
|---------------|---------------------------------------------|
| Framework     | ASP.NET Core (.NET 10), MVC Controllers     |
| Database      | PostgreSQL via Npgsql.EntityFrameworkCore   |
| ORM           | EF Core 10 with code-first migrations       |
| API Docs      | Scalar + built-in OpenAPI (`MapOpenApi`)    |
| Auth          | None                                        |
| Caching       | None                                        |
| Messaging     | None                                        |
| Testing       | NUnit 4 + EF InMemory                       |
| Frontend      | Angular (in `/frontend`)                    |

## Coding Conventions

### C# Style
- Use primary constructors for DI injection (`class Foo(BarService bar)`)
- Use `required` keyword on entity properties that must be set
- Nullable reference types enabled — never suppress warnings without justification
- `ImplicitUsings` enabled — do not add redundant `using System;` etc.
- Prefer `var` for local variables when type is obvious from the right-hand side

### Entities
- Keep entities in `TaskProject.Core/Entities/`
- Use `[MaxLength]` data annotations for string properties stored in PostgreSQL
- Always set `DateTime` fields to UTC; for PostgreSQL use `DateTime.SpecifyKind(..., DateTimeKind.Utc)`

### EF Core
- DbContext: `AppDbContext` in `TaskProject.Core/Data/`
- Run migrations in `Program.cs` on startup (`db.Database.Migrate()`) — already configured
- Generate migrations from the `src/TaskProject.Api` project (it references Core)
  ```
  dotnet ef migrations add <Name> --project src/TaskProject.Core --startup-project src/TaskProject.Api
  ```

### Controllers
- Route pattern: `api/[controller]`
- Use `[ApiController]` + `ControllerBase`
- Return `ActionResult<T>` for endpoints that return data
- Map `ServiceResult` states to HTTP responses:
  - `IsNotFound` → `NotFound()`
  - `!IsSuccess` → `BadRequest(new { error = result.Error })`
  - Success → `Ok(result.Value)` or `NoContent()`

### JSON
- Enums serialized as string names (configured in `Program.cs`) — do not change this

## Testing

- Framework: **NUnit 4** (not xUnit — do not switch)
- Database: **EF InMemory** for unit/service tests
- Test project references both `TaskProject.Core` and `TaskProject.Api`
- Tests live in `tests/TaskProject.Tests/`

```csharp
// Typical service test setup
var options = new DbContextOptionsBuilder<AppDbContext>()
    .UseInMemoryDatabase(Guid.NewGuid().ToString())
    .Options;
var db = new AppDbContext(options);
var service = new TaskService(db);
```

## Running the Project

```bash
# Start PostgreSQL (via Docker Compose)
docker compose up -d

# Run the API
dotnet run --project src/TaskProject.Api

# Run tests
dotnet test

# Run Angular frontend
cd frontend && npm start
```

## Build & Verify

```bash
dotnet build
dotnet test
```

Always ensure `dotnet build` is clean (no warnings treated as errors) before committing.

## What NOT to Do

- Do not add layers (Clean Architecture, MediatR, CQRS) unless explicitly requested — the layered Api+Core structure is intentional for this CRUD-heavy domain
- Do not add FluentValidation — use data annotations for simple validation
- Do not switch testing framework to xUnit
- Do not add auth middleware without being asked
- Do not create `IRepository` abstractions over EF Core — use DbContext directly in services
