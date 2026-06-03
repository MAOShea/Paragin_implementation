# Paragin — Exam Results Analysis

Monolith-first implementation: **Blazor Web App** (Interactive Server) + **Paragin.Shared** domain library.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

No Node.js required.

## Run the app

```bash
cd implementation
dotnet run --project src/Paragin.Web
```

Open the URL shown in the console (typically `http://localhost:5009`). The **Welcome** page is the home route (`/`).

### Run and Debug

**Microsoft VS Code** (with C# Dev Kit): open this repo root → Run and Debug → **Paragin.Web (Debug)** → F5. Breakpoints work. Browser opens at **http://localhost:5009** (`/` = Welcome).

**Cursor:** use **Run Paragin.Web (terminal, no debugger)** or `dotnet run --project src/Paragin.Web` — C# Dev Kit debugging is not licensed in Cursor.

Seed datasets from `user_data/` (InputData_1 and InputData_2) load automatically on startup.

## Run tests

```bash
cd implementation
dotnet test
```

Tests verify pass/fail counts on InputData_1 (68 passed / 384 failed) and grading anchors from Functional Design §6.

## Project layout

| Project | Role |
|---------|------|
| `src/Paragin.Shared` | CSV parsing, grading, P′, rit, validation |
| `src/Paragin.Web` | Blazor UI (Welcome, Configuration, Student results, Item analytics, Data checks) |
| `tests/Paragin.Shared.Tests` | xUnit regression tests |

See [TechnicalDesign.md](../TechnicalDesign.md) and [FunctionalDesign.md](../FunctionalDesign.md).
