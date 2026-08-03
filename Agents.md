# Detailed description of Gridly

Gridly is a self-contained dashboard app for creating shortcuts to apps, services, and websites on a local network. The backend is ASP.NET Core and the frontend is Angular. A build step compiles the Angular app and copies the output into the static files folder, so the published result is a single .NET executable that serves both the API and the frontend — no separate Node/Angular runtime needed to run it.

### How a request flows (step by step)
1. The frontend calls an API endpoint over HTTP.
2. Any route that isn't an API call falls back to serving the frontend's entry page, so client-side routing keeps working on refresh or deep links.
3. Controllers contain no logic of their own. Each one takes the incoming request, wraps it in a request object, and hands it off to a request-dispatch layer.
4. The dispatcher routes each request object to whichever handler is registered for that type (a CQRS-style pattern — every operation has its own dedicated request type).
5. The handler carries out the operation: it calls into a repository (for local data access) or an outbound-call layer (for reaching third-party services), then returns a standard success/failure result back up to the controller.
6. Repositories talk to an embedded local database through a lightweight data-access library rather than a full ORM. Raw queries are kept in a shared constants location; each repository builds a query, runs it through a small shared data-access helper, and maps the raw result into a plain data-transfer shape, which a dedicated mapping layer converts into the domain object handed back to the handler.

### Dependencies
The project relies on a request-dispatch library for the CQRS pattern described above, a lightweight data-access library plus an embedded database engine (so no external database server is required), and a testing framework used by the test project.

### LaunchSettings.json
Defines run profiles per environment (e.g. plain HTTP for local development, and an IIS profile for Windows hosting). Each profile can set its own port and environment variables.

### wwwroot
The static/published output folder for the frontend. A build pipeline compiles the frontend app and moves the compiled output into this folder, then cleans up any intermediate build artifacts. The same pipeline can also wrap the backend build/run steps, so the whole stack can be built and started from one command.

### Commands
One type per operation, representing a single unit of intent (a query or an action) that flows through the request-dispatch layer. Controllers construct these and pass them along; they carry whatever input data the corresponding handler needs. Grouped by feature — reads and writes each get their own type rather than sharing one generic request shape.

### Handlers
One handler per feature area, each responsible for one or more related operations. This is where the actual business logic lives — a handler decides what to do with an incoming request, and delegates the low-level work to a repository or an outbound-call layer rather than doing that work itself. A single handler can own multiple related operations if it makes sense to group them.

### Repositories
The data-access layer for the local embedded database. One repository per entity/table, each defined behind an interface so it can be swapped or mocked in tests. On startup, a dedicated initialization step ensures all required tables exist and seeds any default reference data the app needs to function out of the box.

### EndPoints
The layer responsible for outbound calls to external, third-party services — as opposed to repositories, which only talk to the local database. Each integration point is wrapped behind its own interface, uses a shared HTTP-calling helper to make the request, and a shared serialization helper to convert the response into a usable object.

### Services
Shared, cross-cutting infrastructure used across handlers, repositories, and outbound-call classes. Broadly this includes: a thin wrapper around outbound HTTP calls, an in-memory caching layer (so repeated calls to slow or rate-limited resources aren't repeated unnecessarily), a generic serialization/deserialization helper, the local database connection provider, file-system helpers for asset access, and a secrets-protection service (wrapping ASP.NET Core's Data Protection API) for encrypting sensitive values such as third-party API keys before they're persisted.

### Models / Dtos / Factories
Domain objects (the shapes returned to callers) live separately from data-transfer objects (the flatter shapes that mirror what comes back from a data query, including any joined data). A dedicated mapping layer converts between the two, keeping that conversion logic out of the repositories and handlers themselves.

### Configuration — Rate limiting
Rate limiting policies are registered centrally and applied per-endpoint where needed, generally scoped to whichever endpoints call out to external services with their own usage limits — each such integration gets its own named policy with limits tuned to that external service's constraints, rather than one shared global policy. New outbound integrations should follow the same pattern: define a policy sized to that service's limits, and apply it explicitly to the endpoint(s) that use it. User-facing mutation endpoints that touch secrets (e.g. saving a third-party API key) warrant their own policy too, sized to deter abuse/brute-forcing rather than to an external service's quota.

### Constants
A central place for fixed values that would otherwise be scattered through the code: external/internal endpoint URLs, and raw query fragments used by the repositories.

### Assets
Stores icons, images, and other static assets needed by the app. A dedicated configuration value defines where on disk these are read from; some asset data may also be stored inline in the database rather than on disk.

### Frontend
Organized by responsibility: UI components, a services layer (split between low-level per-resource HTTP calls and higher-level services that use them), plus shared models/DTOs/interfaces/type definitions. Domain concepts that have multiple variants (e.g. different kinds of cards/widgets) are defined as an enum/type list, with room to add new variants without restructuring existing code. Not every variant needs to be fully implemented at once — a variant can be scaffolded (reference data, a type entry, a placeholder in the UI) ahead of its backend implementation being built.

### Tests
Organized to mirror the main source layout — one test area per layer (handlers, repositories, mapping/factory logic, services), with shared test infrastructure (fakes/doubles and result-assertion helpers) factored out so individual tests stay focused on behavior rather than setup.

### Git workflow, branching & pull requests
The repo uses a two-tier branch model, not direct-to-main feature branches:
1. **`sandbox`** is the integration branch. All feature/fix work branches off `sandbox`, not off `main`.
2. Branch names follow the pattern `issue-<number>-<short-kebab-description>` (or `#<number>-<short-description>`), tied to the GitHub issue being worked.
3. While a branch is in progress, periodically merge `sandbox` back into it to stay current, rather than rebasing.
4. Open the pull request against **`sandbox`**, not `main`. A pull request into `main` is automatically rejected unless its source branch is literally `sandbox` — `main` only ever receives `sandbox` as a whole, in a batch, when it's time to release.
5. Every push and every pull request (regardless of target branch) triggers an automated pipeline: install and build the frontend, lint it, run its test suite, then restore/build/test the backend. Treat this as the merge gate — don't consider a change ready for review until it passes. A separate static/security scan also runs against pull requests.
6. Commit messages are short, plain-English summaries of what changed (not a strict conventional-commits format) — write them the way you'd describe the diff to a teammate, and reference the related issue number where relevant.
7. Releases are cut by tagging `main` with a version tag after a `sandbox` → `main` merge, which triggers an automated multi-platform build and publish. Day-to-day feature work never needs to touch this step directly.

When asked to make a change: create a branch off `sandbox` named for the relevant issue, commit with a plain descriptive message, make sure the build/lint/test pipeline would pass, and open the pull request against `sandbox`.

### Pattern summary (quick reference)
- **Backend:** CQRS via a mediator (one request type per operation, one handler each) → repository pattern behind interfaces for all data access → raw query results mapped through a DTO into a domain model by a dedicated mapping layer → dependency injection registered centrally with deliberate lifetimes → rate limiting applied per-endpoint, not globally → secrets encrypted at rest via Data Protection, decrypted only at point of use → folders organized by technical layer, not by feature.
- **Frontend:** standalone components with their own explicit imports → function-based dependency injection → app-wide providers configured once at the root → state held in services behind a subject, exposed as an observable (and bridged to a signal for templates) → components depend on a higher-level state service, never directly on the raw HTTP/endpoint layer → server-side rendering compatible (no unguarded browser-only globals).
- **Shared conventions:** interface-first design wherever something needs to be swappable or mockable in tests; caching in front of anything external or rate-limited; new feature variants may be scaffolded ahead of a full implementation (reference data / type entry / UI placeholder before the real logic exists) rather than requiring everything to land at once.
- **Git:** feature branch off `sandbox` → PR into `sandbox` (never directly into `main`) → automated build/lint/test gate on every push and PR → `sandbox` batched into `main` for releases.

### CI/CD
Automated workflows handle building and testing on every branch, static/security scanning, merge rules, and releases.

### Backend — libraries, patterns, structure
**Libraries:** a mediator library for the CQRS-style command/handler dispatch described above; a lightweight micro-ORM (not a full ORM — no change tracking, no LINQ-to-SQL) paired with a query-builder helper, sitting on top of an embedded database engine rather than a client/server database; an in-memory caching abstraction built into the framework; and a unit test framework with a code-coverage collector.

**Patterns:**
- CQRS via a mediator: every read or write is its own request type, dispatched to exactly one handler — controllers stay free of logic.
- Repository pattern: all database access sits behind an interface per entity, so it can be swapped or mocked in tests.
- Interface-first design more broadly: outbound HTTP integrations and cross-cutting services are also defined behind interfaces, not just repositories.
- Secrets at rest: sensitive values (e.g. third-party API keys) are encrypted via ASP.NET Core's Data Protection API before being persisted, and decrypted only at the point of outbound use — never logged, never returned to a client. The Data Protection key ring is persisted to disk (not left ephemeral), since losing it makes previously-encrypted values permanently unrecoverable.
- DTO → mapper → domain model: raw query results are never handed back directly; they're mapped into DTOs matching the query shape, then converted into domain models by a dedicated mapping layer.
- Centralized dependency injection: everything is registered in one place at startup, with lifetimes chosen deliberately — per-request/connection-scoped for things tied to a single database connection, singleton for stateless shared infrastructure like caching or HTTP helpers.
- Per-endpoint rate limiting: policies are opt-in and attached explicitly to the specific endpoints that need them, not applied globally.

**Structure:** organized by technical layer, not by feature/module — all commands live together, all handlers live together, all repositories live together, and so on, rather than nesting each layer inside a per-feature folder. When adding a new operation, add one item to each relevant layer rather than creating a new feature folder.

### Frontend — libraries, patterns, structure
**Libraries:** a recent version of the framework using its standalone-component model (no shared feature modules); a reactive-state library for observables; a low-level behavior toolkit for things like drag-and-drop interactions; a component/UI primitives library used alongside a general-purpose CSS framework for layout and styling, plus a matching icon set; and a fast unit test runner rather than the framework's older default test runner.

**Patterns:**
- Standalone components: each component declares its own imports directly rather than relying on shared modules.
- Function-based dependency injection: dependencies are pulled in via an injection function at the top of a class rather than through constructor parameters.
- Centralized, functional app configuration: cross-cutting providers (HTTP client, global setup) are configured once at the application root rather than per-module.
- Subject-backed state services: state lives in an injectable service as a private subject, exposed publicly as a read-only observable; newer code also bridges these observables into a signal for direct use in templates.
- Server-side rendering support: the app can render on the server, so code should avoid relying on browser-only globals without guarding for their absence.

**Structure:** split into a low-level "endpoint" layer — one service per resource, responsible only for making the raw HTTP call — and a higher-level service layer that holds state and business logic and calls into the endpoint layer. Components should depend on the higher-level service, not the endpoint layer directly. Shared models, DTOs, interfaces, and type/enum definitions are kept in their own top-level folders, separate from components.

### Useful skills for agents working in this project
- **bootstrap-5** — the frontend styling is built on Bootstrap 5 (plus its icon set). Use this for any layout, component, or styling work in the Angular app rather than reaching for a different CSS approach.
- **analyzing-dotnet-performance** — useful when reviewing or optimizing the backend, especially around caching, database access, and outbound HTTP calls, since those are the app's main performance-sensitive paths.
- **setup-local-sdk** — the project targets a recent .NET version. Use this to test or pin a specific SDK version locally without touching the system-wide install, e.g. when validating against a newer preview release.
- **dotnet-trace-collect** — useful for diagnosing performance issues in a running instance, especially since the app is published self-contained across multiple platforms (Windows, Linux, ARM).
- **dump-collect** — useful for capturing a crash dump if a published, self-contained instance crashes, again across the same range of target platforms.
- **csharp-scripts** — handy for quickly testing a small piece of C# logic in isolation (e.g. a query builder or mapping snippet) before wiring it into the project.
- No dedicated Angular skill is currently available. For frontend work, follow the existing component/service structure and TypeScript conventions already established in the codebase rather than introducing a different pattern.
