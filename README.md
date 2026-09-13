# SutraMind Platform

This folder contains the isolated Windows offline-first implementation. The existing `frontend/` and `backend/` directories remain the Phase 1 React/FastAPI prototype.

## Bootstrap status

The machine currently has .NET SDK 8.0.319 installed, so the initial scaffold targets .NET 8 and builds on this workstation. The target architecture specifies .NET 10 LTS; retarget the projects to `net10.0` and `net10.0-windows` after installing the .NET 10 SDK.

## Projects

- `SutraMind.Desktop`: WPF + MVVM Windows client.
- `SutraMind.Domain`: clinical entities, enums, and domain rules.
- `SutraMind.Application`: use cases and repository contracts.
- `SutraMind.Infrastructure`: EF Core, encrypted SQLite, sync, security, and S3 adapters.
- `SutraMind.Api`: ASP.NET Core cloud API.
- `SutraMind.Tests`: unit and integration tests.

## Completed milestones

- Domain entities mirror the Phase 1 clinical model: studies, sites, memberships, protocols, ethics, medicines, participants, Ayurveda baselines, visits, CRFs, queries, milestones, and master terms.
- Shared contracts cover authentication, study creation, participant enrollment, visits/CRFs, queries, dashboards, and bidirectional synchronization.
- Centralized permission and lifecycle rules protect role boundaries and state transitions before persistence is added.
- EF Core local persistence includes SQLCipher initialization, a versioned `InitialLocalSchema` migration, repositories, and an atomic participant-plus-outbox write test.

The next milestone is connecting the WPF enrollment workflow to this local repository and displaying real pending outbox counts.

## Build and test

```powershell
dotnet build SutraMind.sln
dotnet test SutraMind.sln
```

To validate the WPF project-system state after a clean/rebuild:

```powershell
.\validate-wpf-project.ps1
```

This check confirms that the real desktop project exists and is registered in the solution, and that no generated `_wpftmp.csproj` file is left for VS Code to load.

Development uses synthetic data only. AWS credentials and production secrets must remain outside the repository.
