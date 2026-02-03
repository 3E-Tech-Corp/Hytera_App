# 📡 Hytera Data Core — Analysis & TODO
**Last Updated:** 2026-02-03 | **Maintainer:** Synthia

## Vision
A **clean, stable data access API** that serves as the foundation for all Hytera applications. Multiple frontends (web apps, mobile apps, partner integrations) consume data through API keys and JWT authentication. Modeled after Funtime-Shared's architecture but purpose-built for Hytera's domain.

**This is NOT a patch job on the existing code.** The current controllers are reference material for understanding the data domain. The goal is a clean core built with the right patterns from day one.

## Current Repo State (Reference Only)
- 11 controllers using raw ADO.NET DataSets + stored procedures
- No auth middleware, no response standardization, mixed route patterns
- EF Core DbContext exists but unused for queries
- `OldCode/` folder has legacy .NET Framework controllers
- DB: `DCN` on `HYTSQL`, data lives in stored procs

## Architecture: Clean Data Core

### Design Principles
1. **Stored procedures are the data layer** — Hytera's business logic lives in SQL Server procs. The API is a clean, secure gateway to them.
2. **JWT + API Key dual auth** — Humans get JWT tokens, machines get API keys. Both are first-class.
3. **Multi-tenant from day one** — API keys scope data access. Different clients see different data.
4. **Consistent contracts** — Every endpoint returns the same envelope. Every error is structured. No surprises.
5. **Auditable** — Every data access is logged. Who, what, when, from where.

### Target Architecture
```
┌─────────────────────────────────────────────────┐
│                   Frontends                      │
│  Web App A │ Web App B │ Mobile │ Partner API    │
└──────┬──────┬──────┬──────┬─────────────────────┘
       │      │      │      │
       ▼      ▼      ▼      ▼
┌─────────────────────────────────────────────────┐
│              Hytera Data Core API                │
│                                                  │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐      │
│  │ Auth     │  │ Middleware│  │ Response  │      │
│  │ JWT/Key  │  │ Rate Lim │  │ Envelope  │      │
│  │          │  │ Audit    │  │ Errors    │      │
│  └──────────┘  └──────────┘  └──────────┘      │
│                                                  │
│  ┌──────────────────────────────────────┐       │
│  │         Domain Controllers            │       │
│  │  Auth │ Inventory │ Games │ Assets    │       │
│  │  Apps │ Languages │ Voice │ Admin     │       │
│  └──────────────┬───────────────────────┘       │
│                 │                                 │
│  ┌──────────────▼───────────────────────┐       │
│  │      Data Access Service              │       │
│  │  Stored Proc Executor (Dapper)        │       │
│  │  Connection Management                │       │
│  │  Result Mapping                       │       │
│  └──────────────┬───────────────────────┘       │
└─────────────────┼───────────────────────────────┘
                  │
                  ▼
         ┌────────────────┐
         │  SQL Server     │
         │  HYTSQL / DCN   │
         │  Stored Procs   │
         └────────────────┘
```

## 🔴 Phase 1: Foundation (Build First)

### Auth System (port from Funtime-Shared)
- [ ] **JWT token issuance** — Login endpoint returns JWT with claims (UserId, Role, BPCode, Scopes)
- [ ] **JWT validation middleware** — All endpoints except login require valid token
- [ ] **API key middleware** — `X-API-Key` header for machine clients, maps to tenant/scope
- [ ] **API key management** — CRUD for keys (admin only), each key has: name, scopes, rate limit, expiry
- [ ] **Refresh tokens** — Long-lived refresh + short-lived access token pattern

### Data Access Layer
- [ ] **Dapper-based proc executor** — Replace raw ADO.NET DataSets with Dapper. Typed results, not DataRow parsing.
- [ ] **IDbConnectionFactory** — Pooled connections, not new SqlConnection per request
- [ ] **Generic proc caller** — `Task<T> ExecProcAsync<T>(string proc, object? params)` with auto-mapping
- [ ] **Multi-result support** — Some procs return multiple result sets (QueryMultiple)
- [ ] **Connection string per tenant** — If different clients need different DBs

### Response Standards
- [ ] **Response envelope** — Every response: `{ success: bool, data: T?, message: string?, errors: string[]?, meta: { page, total, timestamp } }`
- [ ] **Error middleware** — Global exception handler, structured error responses, no stack traces in production
- [ ] **Validation** — FluentValidation or DataAnnotations on all request DTOs
- [ ] **HTTP status codes** — Proper use (200/201/400/401/403/404/500), not everything-is-200

### Infrastructure
- [ ] **Health check** — `/health` returning DB connectivity, version, uptime
- [ ] **Swagger/OpenAPI** — Full docs with auth schemes, examples, response types
- [ ] **Logging** — Serilog with structured logging (request/response, proc calls, auth events)
- [ ] **CORS** — Locked to known origins (not AllowAll)
- [ ] **Rate limiting** — Per API key and per user, configurable

## 🟠 Phase 2: Domain Endpoints (Clean Rewrite)

Rewrite each domain using the new foundation. Reference existing controllers for the proc names and contracts.

### Auth Domain (`/auth/*`)
- [ ] `POST /auth/login` — Email/password → JWT (replaces `User/FastLogin`)
- [ ] `POST /auth/login/fast/{userId}` — Quick login by user ID
- [ ] `POST /auth/refresh` — Refresh token → new access token
- [ ] `POST /auth/reset-password` — Request password reset
- [ ] `POST /auth/change-password` — Authenticated password change
- [ ] `GET /auth/me` — Current user profile from token

### Inventory Domain (`/inventory/*`)
- [ ] `GET /inventory/{itemCode}` — Single item lookup
- [ ] `POST /inventory/search` — Search with filters, pagination
- [ ] `POST /inventory/nlu-query` — Natural language inventory search (OpenAI)
- [ ] `PUT /inventory/{itemCode}` — Update item (admin)
- [ ] `GET /inventory/categories` — Item type/category listing

### Games Domain (`/games/*`)
- [ ] `POST /games/scores` — Upload score
- [ ] `POST /games/scores/check` — Check/query scores
- [ ] `GET /games/scores/{eventId}` — Scores by event
- [ ] `GET /games/leaderboard/{eventId}` — Aggregated standings

### Assets Domain (`/assets/*`)
- [ ] `GET /assets/{id}` — Serve file (with caching headers)
- [ ] `GET /assets/{id}/image/{width?}/{height?}` — Serve resized image
- [ ] `GET /assets/{id}/stream` — Stream video
- [ ] `POST /assets/upload` — Upload file (admin, base64 for WAF safety)

### App Management Domain (`/apps/*`)
- [ ] `GET /apps/version/{os}` — Check latest version
- [ ] `POST /apps/version` — Register new version (admin)
- [ ] `GET /apps/languages/{code}` — Get language pack
- [ ] `GET /apps/voicesets` — List voice sets
- [ ] `GET /apps/voicesets/{code}` — Get specific voice set
- [ ] `POST /apps/roc/link` — Link new ROC

### Admin Domain (`/admin/*`)
- [ ] `GET /admin/users` — List users (paginated)
- [ ] `PUT /admin/users/{id}` — Update user
- [ ] `GET /admin/api-keys` — List API keys
- [ ] `POST /admin/api-keys` — Create API key
- [ ] `DELETE /admin/api-keys/{id}` — Revoke API key
- [ ] `GET /admin/audit-log` — Query audit trail

## 🟡 Phase 3: Production Readiness

- [ ] **Audit logging** — Middleware logs every request: who, what endpoint, params, response code, duration
- [ ] **Webhook system** — Register webhooks, fire on inventory/score changes
- [ ] **Caching** — In-memory or Redis for inventory, versions, languages (configurable TTL)
- [ ] **Deployment** — GitHub Actions workflow (IIS deploy, DB migration runner)
- [ ] **Monitoring** — Health dashboard, error rate tracking, response time metrics
- [ ] **Documentation site** — API docs for external consumers (partner integration guide)

## 🟢 Phase 4: Expansion

- [ ] **Multi-database routing** — Different tenants → different SQL Server instances
- [ ] **Event sourcing** — Track all data changes for replay/audit
- [ ] **GraphQL layer** — Optional GraphQL endpoint for flexible querying
- [ ] **SDK generation** — Auto-generate TypeScript/C# client SDKs from OpenAPI spec

## File Structure (Target)
```
Backend/API/
├── Auth/
│   ├── JwtService.cs
│   ├── ApiKeyMiddleware.cs
│   └── ApiKeyService.cs
├── Controllers/
│   ├── AuthController.cs
│   ├── InventoryController.cs
│   ├── GamesController.cs
│   ├── AssetsController.cs
│   ├── AppsController.cs
│   └── AdminController.cs
├── Data/
│   ├── IDbConnectionFactory.cs
│   ├── DbConnectionFactory.cs
│   ├── IProcExecutor.cs
│   └── DapperProcExecutor.cs
├── Middleware/
│   ├── ErrorHandlingMiddleware.cs
│   ├── AuditMiddleware.cs
│   ├── RateLimitMiddleware.cs
│   └── ResponseEnvelopeMiddleware.cs
├── Models/
│   ├── Requests/
│   ├── Responses/
│   ├── Domain/
│   └── ApiEnvelope.cs
├── Services/
│   ├── InventoryService.cs
│   ├── GameService.cs
│   ├── AssetService.cs
│   └── AppService.cs
├── Scripts/
│   └── archives/
├── Program.cs
└── appsettings.json
```

## Technical Notes
- **DB:** SQL Server on `HYTSQL`, database `DCN`. All business logic in stored procs.
- **Asset storage:** `D:\Docvault\www` on the Hytera server
- **Contact:** Tomas Rosales (tomas.rosales@hytera.us)
- **Existing procs:** `psp_CheckSAP` (login), others TBD — need to catalog all procs in DCN
- **Model after:** Funtime-Shared auth patterns, FXNotification API key patterns
