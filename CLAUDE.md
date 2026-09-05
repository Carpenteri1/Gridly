# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project overview

Gridly is a self-contained dashboard app: an ASP.NET Core MVC backend (`net10.0`, single `Gridly.csproj` at the repo root) serving an Angular SPA (`Gridly-Client/`) that's compiled and copied into `wwwroot/`. It ships as self-contained single-file executables for win-x64/linux-x64/linux-arm/linux-arm64 with no separate .NET/Angular install required on the target machine.

## Branch workflow

**PRs into `main` are only accepted from a branch literally named `sandbox`** — enforced by `.github/workflows/enforce_sandbox_merge.yml`, which fails any PR into `main` whose head ref isn't `sandbox`. Do development work on `sandbox` (or a branch merged into it) before it reaches `main`; a feature branch opened directly against `main` will fail CI on that check alone.

## Commands

### Backend (.NET) — run from repo root
- `dotnet restore`
- `dotnet build --configuration Release --no-restore`
- `dotnet test Gridly.csproj --configuration Release --no-build` — run all tests (the same `Gridly.csproj` is both the web app and the xUnit test project, via `<IsTestProject>true</IsTestProject>`)
- Single test: `dotnet test Gridly.csproj --filter "FullyQualifiedName~CardHandlerTests.MethodName"`
- `dotnet run` — serves at `http://localhost:7575` (see `Kestrel:Endpoints:Http:Url` in `appsettings.json`)

### Frontend (Angular) — run from `Gridly-Client/`
- `npm install`
- `npm run build` (`ng build`)
- `npm run lint` (`ng lint`; CI runs it as `ng lint -- --max-warnings=0`)
- `npm test` (Jest, via `jest-preset-angular`, not Karma/Jasmine; CI adds `--runInBand --coverage`)
- Single test: `npx jest src/app/components/card/card.component.spec.ts` or `npx jest -t "test name"`
- `npm run watch` — `ng build --watch --configuration development`

### Combined local dev (gulp, run from `Gridly-Client/`)
Actual task names in `gulpfile.js`:
- `npx gulp build-angular-net-debug` — `ng build`, copy output into `../wwwroot`, then `dotnet build --configuration Debug`
- `npx gulp build-angular-debug` — `ng serve` (hot-reload dev server on `:4200`)
- `npx gulp build-net-debug` — `dotnet restore` + `dotnet build --configuration Debug`

Note: `npm run start` and `npm run start-angular` invoke gulp tasks `build-angular-net` / `build-angular`, which don't exist in `gulpfile.js` (only the `-debug` variants do) — those two npm scripts currently error if run as-is.

## Architecture

### Backend: Controller → MediatR command → Handler → Repository → SQLite (Dapper)
- `Controllers/*Controller.cs` are thin: each action builds a MediatR request and calls `mediator.Send(...)`. No business logic lives in controllers.
- `Command/*.cs` define one MediatR request per operation (e.g. `SaveCardCommand : CardModel, IRequest<IResult>`).
- `Handlers/*Handler.cs` implement `IRequestHandler<TCommand, TResult>`, often for several related commands in one handler class (e.g. `CardHandler` handles save/get/edit/batch-edit/delete for cards), injecting repositories/services via primary constructors. This is where the actual business logic lives.
- `Repositories/*Repository.cs` wrap Dapper access to a single shared SQLite database (`Assets/Db/Gridly.db`; connection string `GridlyDb` in `appsettings.json`). They use `Data/DbCommandRunner` (generic `Execute`/`Select`/`SelectMany` helpers over `IDbConnection`) with raw SQL kept as constants in `Constants/QueryStrings.cs`, composed via Dapper's `SqlBuilder` for joins/where clauses.
- `Data/DbInitializer` creates the `Card`, `IconsConnected`, `Icon`, `Settings` tables on startup with `CREATE TABLE IF NOT EXISTS` — there are no formal EF-style migrations.
- `Factories/*Factory.cs` map DB DTOs (`Dtos/*DtoModel.cs`) to domain `Models/*Model.cs` and back.
- `Services/` hold cross-cutting singletons wired in `Program.cs`: DB connection factory (`IDbConnectionServices`), in-memory caching, HTTP client wrapper, icon/file storage (`IFileService`), a generic `IDataConverter<T>`.
- `EndPoints/` (currently just `Version`) is a smaller, separate request-handling pattern alongside Command/Handler — not unified with it; don't assume every feature follows the Command/Handler shape.
- A custom token-bucket rate limiter (`Configuration/TokenBucketRateLimiter.cs`) is applied to the default MVC route (`RequireRateLimiting("fixed")`).
- All DI registration happens in `Program.cs` — new repositories/services/handlers need to be registered there.

### Frontend: Angular SPA built into the .NET app's `wwwroot`
- Feature-organized under `Gridly-Client/src/app/components/` (`grid`, `card`, `header`, `dialogs`), with matching services under `src/app/services/*_services/`.
- The .NET and Angular projects are independent (`Gridly.csproj` vs. `Gridly-Client/package.json`) and are glued together only by copying Angular's build output (`dist/Gridly-Client/browser`) into `wwwroot/`: `release.yml` does this with a plain `mv` in CI, local dev uses the gulp tasks above.
- `Program.cs` serves `wwwroot` as static files and falls back to `index.html` for Angular's client-side routing (`MapFallbackToFile("index.html")`).

### Tests
- Backend: xUnit under `Tests/`, mirroring source layout (`Tests/Handlers`, `Tests/Repositories`, `Tests/Services`, `Tests/Factories`, `Tests/Helpers`), plus shared fakes/assertions in `Tests/Infrastructure/` (`TestDoubles.cs`, `ResultAssertions.cs`).
- Frontend: Jest specs colocated as `*.spec.ts` next to the code they test.

### CI
- `Build_Test_Branches.yml` runs on every push/PR to any branch, in this order: `npm install` → `npm run build` → `npm run lint` → `npm test` (all in `Gridly-Client/`), then `dotnet restore` → `dotnet build --configuration Release` → `dotnet test` at the root. Match this order locally before pushing.
- `enforce_sandbox_merge.yml` — see Branch workflow above.
- `release.yml` — tag-triggered (`v*`): builds the Angular app, moves it into `wwwroot`, then produces self-contained single-file `dotnet publish` outputs for win-x64/linux-x64/linux-arm/linux-arm64, zipped as GitHub release assets.
- Dependabot opens per-package version bumps for the Angular ecosystem in `Gridly-Client`. Because `@angular/*`, `@angular/cdk`, `@angular/material`, `@angular-devkit/build-angular`, and `@angular/cli` are version-locked to each other via peer dependencies, a single-package bump routinely fails CI with an `ERESOLVE` peer-dependency conflict — the whole family needs to move together for the bump to actually install.
