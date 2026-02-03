# 📡 Hytera App — Analysis & TODO
**Last Updated:** 2026-02-03 | **Maintainer:** Synthia

## Current State
- **Repo:** 3E-Tech-Corp/Hytera_App
- **Stack:** .NET 8 + Dapper/ADO.NET + SQL Server (stored procedures)
- **DB:** `DCN` on `HYTSQL` server
- **Status:** API-only backend, early stage. Will evolve into a common data provider for Hytera.

## Architecture
- Backend-only .NET 8 API (no frontend in this repo)
- All data access via stored procedures (`psp_CheckSAP`, etc.) through `DatabaseService`
- EF Core `HyteraDbContext` exists but only for schema reference — actual queries use raw ADO.NET DataSets
- OldCode/ folder has legacy .NET Framework controllers (reference only)

## Current Endpoints
| Route | Method | Purpose |
|-------|--------|---------|
| `User/FastLogin/{userId}` | GET/POST | Login via stored proc `psp_CheckSAP` |
| `User/ResetPassword/{email}` | GET/POST | Password reset |
| `Api/Inventory/{itemCode}` | GET/POST | Inventory lookup/search |
| `api/nlu/inventory` | POST | NLU-based inventory query (OpenAI GPT) |
| `Game/CheckScore` | POST | Check game scores |
| `Game/UploadScore` | POST | Upload game scores |
| `Asset/Image/{id}` | GET | Serve images (with resize) |
| `Asset/File/{id}` | GET | Serve files |
| `Asset/streamvideo/{id}` | GET | Stream video |
| `App/CheckNewVersion/{os}` | GET/POST | App version checking |
| `App/Language/{code}` | GET/POST | Language pack management |
| `App/Voiceset` | GET/POST | Voice set management |
| `App/LinkNewROC/{appId}` | GET/POST | ROC linking |

## 🟠 High — Vision: Common Data Provider

Feng's plan: Expand into a **centralized data API for Hytera**, similar to how Funtime-Shared serves all pickleball sites. Multiple frontends (web, mobile, partner apps) consume data via API keys and JWT.

### Phase 1: Auth & Security Foundation (copy from Funtime-Shared)
- [ ] **JWT authentication** — Currently NO auth middleware. Port JWT setup from Funtime-Shared.
- [ ] **API key middleware** — For machine-to-machine access (partner apps, frontends)
- [ ] **Rate limiting** — Protect endpoints from abuse
- [ ] **CORS configuration** — Currently `AllowAll` — lock down to known origins
- [ ] **Secrets management** — Move connection strings and API keys out of appsettings.json

### Phase 2: API Standardization
- [ ] **Consistent route structure** — Currently mixed (`User/`, `Api/`, `Game/`, `App/`, `Asset/`). Standardize.
- [ ] **Response envelope** — Standard `{ success, data, message, errors }` wrapper
- [ ] **Pagination** — For inventory and score listing endpoints
- [ ] **Swagger/OpenAPI docs** — Already has Swagger in dev, expand with proper XML docs
- [ ] **Versioning** — API versioning (v1/v2) for breaking changes

### Phase 3: Data Provider Expansion
- [ ] **Multi-tenant support** — Different API keys get different data scopes
- [ ] **Webhook system** — Notify external systems on inventory changes, score updates
- [ ] **Audit logging** — Track who accessed what data and when
- [ ] **Caching layer** — Redis or in-memory for frequently accessed data (inventory, versions)

## 🟡 Medium — Feature Gaps
- [ ] **User management CRUD** — Currently only login/password reset. Need create, update, list, deactivate.
- [ ] **Inventory CRUD** — Only read/search exists. Need admin create/update/delete.
- [ ] **Role-based access** — `UserRole` and `BPRoleName` exist in DB but no authorization enforcement
- [ ] **File upload** — Assets table exists but no upload endpoint in new code
- [ ] **NLU improvements** — InventoryNluController uses OpenAI but API key is empty in config

## 🟢 Low — Nice to Have
- [ ] **Health check endpoint** — `/health` for monitoring
- [ ] **Background jobs** — Inventory sync, stale score cleanup
- [ ] **Export endpoints** — CSV/Excel export for inventory, scores
- [ ] **Dashboard API** — Summary stats for admin frontend

## Technical Notes
- **DB:** SQL Server on `HYTSQL`, database `DCN`. Auth via `sa` (needs service account).
- **Stored procs:** `psp_CheckSAP` (login), others TBD — most logic lives in the DB
- **Asset storage:** `D:\Docvault\www` on the server, served via AssetController
- **OldCode/:** Legacy .NET Framework reference — don't modify, use for understanding original API contracts
- **OpenAI:** Key configured but empty — needs setting for NLU inventory feature
