# StockChatLive .NET 10.0 Upgrade Tasks

## Overview

This document tracks the execution of the StockChatLive project upgrade from .NET 6.0 to .NET 10.0 (LTS). The project will be upgraded in one atomic operation.

**Progress**: 2/3 tasks complete (67%) ![67%](https://progress-bar.xyz/67)

---

## Tasks

### [✅] TASK-001: Verify prerequisites
**References**: Plan §Migration Strategy - Step 1: Prerequisites

- [✅] (1) Verify .NET 10 SDK installed via `dotnet --list-sdks`
- [✅] (2) .NET 10.0.xxx SDK present (**Verify**: .NET 10.0.102 SDK found)

---

### [✅] TASK-002: Atomic framework and package upgrade
**References**: Plan §Migration Strategy - Steps 2-6, Plan §Package Update Reference, Plan §Breaking Changes Catalog

- [✅] (1) Update `StockChatLive\StockChatLive.csproj` TargetFramework from net6.0 to net10.0
- [✅] (2) TargetFramework updated to net10.0 (**Verified**: Updated to net10.0)
- [✅] (3) Update `Microsoft.AspNetCore.Authentication.JwtBearer` package reference from version 6.0.0 to 10.0.0 in StockChatLive.csproj
- [✅] (4) Package reference updated to version 10.0.0 (**Verified**: Updated to 10.0.0)
- [✅] (5) Update SignalR JavaScript client CDN reference in `Pages/Index.cshtml` from version 5.0.11 to 10.0.0
- [✅] (6) SignalR client CDN URL updated (**Verified**: Updated to 10.0.0)
- [✅] (7) Run `dotnet restore` in StockChatLive directory
- [✅] (8) All dependencies restored successfully (**Verified**: Restore complete in 0.9s)
- [✅] (9) Build solution and fix all compilation errors per Plan §Breaking Changes Catalog (focus on JWT TokenValidationParameters and SignalR configuration if needed)
- [✅] (10) Solution builds with 0 errors (**Verified**: Build succeeded in 4.5s with 0 errors)

---

### [▶] TASK-003: Final commit
**References**: Plan §Source Control Strategy - Single Commit Approach

- [▶] (1) Commit all changes with message: "Upgrade StockChatLive to .NET 10\n\n- Update target framework: net6.0 → net10.0\n- Update Microsoft.AspNetCore.Authentication.JwtBearer: 6.0.0 → 10.0.0\n- Update SignalR JavaScript client: 5.0.11 → 10.0.0\n- Fix compilation errors per breaking changes catalog\n- Solution builds successfully with 0 errors"

---
