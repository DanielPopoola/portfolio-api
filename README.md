# Portfolio Management API

A REST API (ASP.NET Core Web API, .NET 8) where investors create portfolios, record Buy and Sell transactions against assets, and read back holdings, profit and loss, and allocation. Built with EF Core (code-first, migrations) on SQL Server.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0). The repo pins the SDK with `global.json`, so it also works if a newer SDK is installed.
- SQL Server. The default connection string uses **LocalDB** (installed with Visual Studio's data workload, or standalone as SQL Server Express LocalDB).
- The EF Core command-line tool:

```bash
dotnet tool install --global dotnet-ef --version 8.0.*
```

## Connection string

It lives in `src/PortfolioApi/appsettings.json` under `ConnectionStrings:Default`:

```json
"Default": "Server=(localdb)\\MSSQLLocalDB;Database=PortfolioApi;Trusted_Connection=True;TrustServerCertificate=True"
```

It uses Windows authentication, so there is no password in the repository. To use another SQL Server instance, change the `Server` value.

## Create the database

From the repository root:

```bash
dotnet ef database update --project src/PortfolioApi
```

This applies the committed migrations (`src/PortfolioApi/Migrations`) and creates the `PortfolioApi` database.

## Run

```bash
dotnet run --project src/PortfolioApi
```

The console prints the listening address. In Development, Swagger UI is available at `/swagger`.

To exercise the API, open `requests.http` (Visual Studio or the VS Code REST Client extension) and set `@host` to the printed address. It contains the twelve steps of the worked example in order, followed by requests for the main error cases. Run the worked example against an empty database:

```bash
dotnet ef database drop --project src/PortfolioApi --force
dotnet ef database update --project src/PortfolioApi
```

## Endpoints

| Method and route | Success | Errors |
|---|---|---|
| `POST /api/assets` | 201 | 400, 409 duplicate symbol |
| `GET /api/assets?type=&search=` | 200 | 400 |
| `GET /api/assets/{symbol}` | 200 | 404 |
| `PUT /api/assets/{symbol}/price` | 200 | 400, 404 |
| `POST /api/portfolios` | 201 | 400, 409 same name for same owner |
| `GET /api/portfolios?ownerEmail=` | 200 | 400 |
| `GET /api/portfolios/{id}` | 200 | 404 |
| `DELETE /api/portfolios/{id}` | 204 | 404, 409 has transactions |
| `POST /api/portfolios/{id}/transactions` | 201 | 400, 404, 409 insufficient holdings or concurrent change |
| `GET /api/portfolios/{id}/transactions?symbol=&type=&from=&to=&page=&pageSize=` | 200 | 400, 404 |
| `GET /api/portfolios/{id}/holdings` | 200 | 404 |
| `GET /api/portfolios/{id}/summary` | 200 | 404 |

## Project structure

```
src/PortfolioApi/
├── Controllers/   HTTP in, DTO out; no business logic, no DbContext
├── Services/      business rules, one service (plus interface) per area
├── Data/          AppDbContext, entity configurations, Migrations
├── Domain/        entities, enums, and HoldingMath (pure formulas)
├── Dtos/          request and response records
├── Exceptions/    AppException, NotFoundException, ConflictException
├── Middleware/    GlobalExceptionHandler (ProblemDetails)
└── Common/        MoneyMath (the only place rounding happens), Symbols, JSON converter
```

Controllers depend on service interfaces, services depend on `AppDbContext`, and the formulas in `HoldingMath` depend on nothing, so each layer can change without touching the others.

## Design decisions

- **Layering without a repository.** `DbContext` already acts as a unit of work and repository, and the atomic checks need direct access to it, so an extra repository layer would only add indirection. Services sit behind interfaces and are registered through dependency injection (`DbContext` is Scoped).
- **The database decides uniqueness.** Duplicate symbols, duplicate portfolio names and deleting a portfolio that has transactions are enforced by a unique index and a `Restrict` foreign key. The service catches the SQL Server error (2601/2627 for duplicates, 547 for foreign keys) and turns it into a 409. A check-then-insert would have a race between the check and the write.
- **Optimistic concurrency with `RowVersion`.** Each `Holding` has a row-version token, and the transaction and the holding update are saved in one `SaveChangesAsync`. If two requests change the same holding, the second matches zero rows, EF throws `DbUpdateConcurrencyException`, and the API returns 409. I chose this over a guarded `ExecuteUpdateAsync` (also atomic, but it needs an explicit database transaction to keep the receipt insert in step) and over `REPEATABLE READ` (read locks held longer, deadlocks as the failure mode, and no protection for a first buy where no row exists yet). The trade-off is that the losing request gets "concurrent change, retry" rather than "insufficient holdings". A `CHECK (Quantity >= 0)` constraint is a last safety net. A simultaneous first buy is caught by the unique index on `(PortfolioId, AssetId)`.
- **Money maths is pure and rounds once.** `HoldingMath` holds the formulas (average cost after a buy includes the fee; realized P&L on a sell is cash in minus quantity times average cost) and `MoneyMath` is the only place that rounds, with `MidpointRounding.AwayFromZero`. Everything is computed from exact `decimal` values and rounded only when stored (average cost, 4 places) or returned (money and percentages, 2 places). No `double` or `float` anywhere.
- **Holdings is a single SELECT.** The query filters, sorts and projects through the `Asset` navigation, so it is one statement joining `Holdings` and `Assets`. The derived fields are computed in C# afterwards, because SQL cannot call the rounding helper. An extra existence query runs only when the result is empty, to tell "no holdings" (200, `[]`) from "no such portfolio" (404). See `evidence/holdings-sql.txt`.
- **Summary reads closed positions for realized P&L.** A sold-out holding keeps its row and its realized profit. The summary loads every holding row of the portfolio, sums realized P&L over all of them, and uses only open rows (quantity above 0) for market value, cost basis and allocation.
- **Errors are uniform.** One `IExceptionHandler` converts the typed exceptions into RFC 7807 `ProblemDetails`. Anything unexpected is logged on the server and returned as a generic 500 with no stack trace. Validation uses data annotations, and `[ApiController]` returns the 400 `ValidationProblemDetails` before the service runs.
- **Reads** use `AsNoTracking` with `Select` projections, and every EF call receives the `CancellationToken`.

## Limits

- The transaction list returns `realizedPnl` as `null`: the specified data model has no column for it, so a past sale's profit exists only in the create response and in the holding's running total. Storing it per transaction would be the next improvement.
- A concurrent update on the same holding is reported as a 409 for the client to retry. There is no automatic retry.
- Updating an asset's price is last-write-wins; `Asset` has no concurrency token.
- A failed insert still consumes an identity value, so ids can have gaps.
- There are no automated tests yet. Behaviour was verified manually against the worked example and the acceptance scenarios (see `evidence/`). The formulas in `HoldingMath` are pure functions, which makes them the natural first xUnit targets.
- No authentication, currency conversion or front end (out of scope).

## Evidence

The `evidence/` folder contains:

- `acceptance-run.txt`: results of the worked example and the acceptance scenarios
- `holdings-sql.txt`: the logged SQL for the holdings request (a single SELECT)
- `concurrency-test.txt`: five simultaneous sells of the same holding (one 201, four 409)
- `500-response.txt`: a 500 response with its server log entry
