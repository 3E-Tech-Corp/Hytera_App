# Hytera App

## Overview
Full-stack web application for Hytera US Inc. Users can login, check inventory, orders and RMA statuses.

## Tech Stack
- **Backend**: ASP.NET Core 8, Entity Framework Core, MS SQL Server 2014
- **Frontend**: React 18, Vite, TailwindCSS, PWA (VitePWA)

## Directory Structure
- `/Backend/API` - ASP.NET Core Web API
- `/Backend/API/Scripts` - Database migration scripts
- `/Frontend` - React SPA
- `/Documentation` - Feature and Function Documentations

## Database Migrations
Store all database migration scripts in `/Backend/API/Scripts/` with naming convention:
- `Migration_XXX_FeatureName.sql` (e.g., `Migration_014_SkillGroups.sql`)
- Scripts should be idempotent (safe to run multiple times)
- Include `IF NOT EXISTS` checks for tables and columns
- Use `PRINT` statements for progress logging

## Database Best Practices
- **Prefer stored procedures** over complex inline EF Core queries whenever possible
- Stored procedures avoid EF Core query generation issues (e.g., SQL syntax errors with CTEs, `Contains()` on lists)
- Stored procedures provide better performance for complex operations
- Create stored procedures in migration scripts using  `If exists then Alter Else CREATE PROCEDURE`
- Call stored procedures from C# using `_context.Database.ExecuteSqlRawAsync()` or `FromSqlRaw()`
- Use stored procedures especially for:
  - Bulk operations (merge, delete multiple records)
  - Complex joins or subqueries
  - Operations with multiple steps that should be atomic
  - Queries with dynamic filtering on lists of IDs

## Commands
- Backend: `dotnet run` in `/Backend/API`
- Frontend: `npm run dev` in `/Frontend`
- Build Frontend: `npm run build` in `/Frontend`

## Deployment
- **Production URL**: https://app.Hytera.net
- **Backend**: Deployed as IIS virtual application at `/api` path
  - Controllers use `[Route("[controller]")]` without `/api` prefix
  - IIS virtual application provides the `/api` prefix automatically
  - Frontend calls `/api/agegroups` → IIS routes to virtual app → controller handles `/agegroups`
- **Frontend**: Static files served from IIS root

## Mobile-First PWA Design
This app is designed to be installed as a Progressive Web App (PWA) on mobile devices:

### Design Requirements:
- **Mobile-first**: Design for mobile screens first, then scale up to desktop
- **Touch-friendly**: Large tap targets (min 44x44px), proper spacing between interactive elements
- **Responsive**: Use TailwindCSS responsive classes (`sm:`, `md:`, `lg:`) appropriately
- **Fast loading**: Minimize bundle size, lazy load where possible
- **Offline-capable**: Service worker handles caching (configured in VitePWA)

### UI Guidelines:
- Bottom navigation for primary actions on mobile
- Swipe gestures where appropriate
- Pull-to-refresh patterns
- Native-like transitions and animations
- Avoid hover-only interactions (touch devices don't have hover)
- Use `min-h-screen` and proper viewport handling

### PWA Configuration:
- Manifest configured in `vite.config.js` (VitePWA plugin)
- Icons: `/public/icon-192.png` and `/public/icon-512.png`
- Theme color: `#3b82f6` (blue)
- Display mode: `standalone` (appears like native app)
     
### Usage Flow

1. **Configure Division**: Set division settings, match formats
2. **Create Phases**: Use a template OR add phases manually (Pool Play → Semifinals → Finals)
3. **Configure Pools**: If pool play, create multiple pools
4. **Set Advancement Rules**: Define how units advance between phases
5. **Generate Schedules**: Create placeholder-based schedules for each phase
6. **Assign Courts**: Assign court groups to phases/divisions
7. **Calculate Times**: Generate estimated start times
8. **Preview**: View complete schedule with placeholders
9. **Drawing**: Fill phase 1 slots with actual units
10. **Execute**: As matches complete, winners/losers advance automatically

## Phase Template System (Migration 137+)

Pre-built tournament format templates that TDs can select and apply to divisions, treating phases as composable "Lego blocks" with clear input/output slots.

### Core Concepts

**Phases as Composable Units**: Each phase has:
- **Incoming Slots**: Units entering this phase (from seeding or previous phase)
- **Exiting Slots**: Units leaving this phase (with final ranking/position)
- **Internal Logic**: How incoming → exiting (bracket, round robin, etc.)

**Phase Chaining**: Phases connect when `Phase A.ExitingSlotCount == Phase B.IncomingSlotCount`

### PhaseTemplate Entity

```csharp
public class PhaseTemplate
{
    public int Id { get; set; }
    public string Name { get; set; }           // "8-Team Single Elimination"
    public string Category { get; set; }       // SingleElimination, DoubleElimination, RoundRobin, Pools, Combined
    public int MinUnits { get; set; }          // Minimum supported units
    public int MaxUnits { get; set; }          // Maximum supported units
    public int DefaultUnits { get; set; }      // Default unit count
    public bool IsSystemTemplate { get; set; } // Pre-built vs user-created
    public string StructureJson { get; set; }  // Full structure definition
}
```

### System Templates

| Template | Description | Units |
|----------|-------------|-------|
| Single Elimination (Flexible) | Auto-sizes bracket with byes | 4-32 |
| 4-Team Single Elim | SF → F (+ 3rd place) | 4 |
| 8-Team Single Elim | QF → SF → F | 8 |
| 16-Team Single Elim | R16 → QF → SF → F | 16 |
| Round Robin (4/8 teams) | All play all | 4, 8 |
| 2 Pools + Semifinals + Finals | Pool A/B → SF → F | 8 |
| 4 Pools + Bracket | Pools → QF → SF → F | 16 |
| 8-Team Double Elimination | WB + LB → Grand Final | 8 |
| Pools + Bracket (Flexible) | Auto-pools → bracket | 6-32 |

### API Endpoints

**PhaseTemplatesController** (`/phasetemplates`):
- `GET` - List all active templates
- `GET /for-units/{unitCount}` - Get templates suitable for unit count
- `GET /{id}` - Get template with full structure
- `GET /category/{category}` - Get templates by category
- `POST` - Create custom template (Admin)
- `PUT /{id}` - Update template (Admin)
- `DELETE /{id}` - Soft delete template (Admin)
- `POST /preview` - Preview what applying a template would create
- `POST /{templateId}/apply/{divisionId}` - Apply template to division
- `POST /manual-exit-assignment` - TD manually assigns exit slot
- `GET /{phaseId}/exit-slots` - Get exit slot status
- `POST /{phaseId}/process-byes` - Process all bye encounters

### Stored Procedures (Migration 137)

- `sp_ManuallyAssignExitSlot` - TD override for exit slot assignment
- `sp_ProcessByeEncounters` - Auto-complete bye encounters and advance winners
- `sp_ApplyPhaseTemplate` - Apply template to division (helper)

### Frontend Components

**TemplateSelector.jsx** (`/Frontend/src/components/tournament/TemplateSelector.jsx`):
- Template browsing by category
- Unit count filtering (shows suitable templates)
- Live preview of tournament structure
- Advancement rules visualization
- One-click template application

**PhaseManager.jsx** Integration:
- "Use Template" button (purple) for template-based creation
- "Add Phase" button (blue) for manual phase creation
- Template selector modal with preview

## Shared Authentication (Hytera-Shared)
This project uses shared authentication from the Hytera-Shared repository:
- **UserId**: All Users.Id values come from the shared auth service (no local IDENTITY)
- **JWT Tokens**: Tokens are issued by shared auth and validated locally with `sites[]` claim
- **Cross-site tracking**: Same UserId across all Hytera.Net Sites
- **Site-specific roles**: Each site maintains its own Role (User/Admin)

### Auth Flow:
1. Frontend calls shared auth API for login/register
2. Shared auth returns JWT token with UserId and site claims
3. Frontend calls local `/auth/sync` to create/update local user record
4. Local backend validates token and maintains site-specific data

### Configuration:
- Backend: `appsettings.json` → `SharedAuth:BaseUrl`
- Frontend: `VITE_SHARED_AUTH_URL` environment variable

### Hytera-Shared Integration
Reference: https://github.com/LegalDragon/Hytera-Shared (branch: `claude/debug-prompt-length-yPtFk`)

#### Architecture Components:
- **Backend**: .NET 8 API - centralized authentication, user profiles, site memberships, Stripe payments
- **Frontend**: `@Hytera/ui` npm package - API clients, auth hooks, pre-built UI components
- **Database**: MS SQL Server (FuntimeIdentity) - auth, profiles, memberships, payments

#### Authentication Methods:
- Email/password credentials
- Phone-based OTP via Twilio
- Account linking across auth types
- OAuth providers

#### Frontend Integration Steps:
1. Set environment variables for Identity API URL and site key
2. Create init file calling `initFuntimeClient()` with token retrieval and unauthorized handlers
3. Import shared styles and initialize before app renders
4. Use hooks: `useAuth()`, `useSites()`, `usePayments()`
5. Use components: Button, Input, AuthForm, Avatar, SkillBadge, SiteBadge

#### API Endpoints (AuthController):
- Registration and login (public)
- OTP request and verification
- Account linking
- User info retrieval
- Token validation
- All authenticated endpoints require JWT bearer token

#### Security Requirements:
- HTTPS required in production
- JWT signing key minimum 32 characters
- BCrypt password hashing
- Rate limiting: 5 OTP attempts per 15 minutes (configurable)

## Related Repositories
@https://github.com/LegalDragon/Funtime-Shared
@https://github.com/LegalDragon/Casec-project

## Shared Services and Base Classes
      