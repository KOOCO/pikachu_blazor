# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build, Test, and Development Commands

### Primary Development Commands
- **Build**: `dotnet build src/Kooco.Pikachu.Blazor --configuration Release`
- **Test**: `dotnet test src/Kooco.Pikachu.Blazor --no-build`
- **Run**: `dotnet run --project src/Kooco.Pikachu.Blazor`
- **Publish**: `dotnet publish src/Kooco.Pikachu.Blazor --configuration Release --output published`

### Database Migration
- **Migration**: Use `src/Kooco.Pikachu.DbMigrator` project for database migrations
- **Run migrator**: `dotnet run --project src/Kooco.Pikachu.DbMigrator`

### Solution-wide Commands
- **Build entire solution**: `dotnet build Kooco.Pikachu.sln`
- **Restore packages**: `dotnet restore`

## Architecture Overview

This is an **ABP Framework** e-commerce application built with **Blazor Server** following **Domain-Driven Design (DDD)** principles.

### Core Project Structure
```
src/
├── Kooco.Pikachu.Blazor/              # Main Blazor Server UI application
├── Kooco.Pikachu.Blazor.Store/        # Store-specific Blazor components
├── Kooco.Pikachu.Application/         # Application service layer
├── Kooco.Pikachu.Application.Contracts/  # Service contracts/DTOs
├── Kooco.Pikachu.Domain/              # Domain entities and business logic
├── Kooco.Pikachu.Domain.Shared/       # Shared domain constants/enums
├── Kooco.Pikachu.EntityFrameworkCore/ # Data access layer
├── Kooco.Pikachu.HttpApi/             # Web API controllers
├── Kooco.Pikachu.HttpApi.Client/      # HTTP client proxies
└── Kooco.Pikachu.DbMigrator/          # Database migration utility
```

### Key Business Domains
- **Group Buys**: Core group purchasing functionality
- **Orders**: Order management and processing
- **Members**: User/customer management
- **Products**: Product catalog and inventory
- **Campaigns**: Marketing campaigns and promotions
- **Payments**: Payment gateway integrations (ECPay, Line Pay, China Trust)
- **Logistics**: Shipping and delivery management
- **Shopping Credits**: Store credit system
- **Discount Codes**: Promotional codes and discounts

### Framework-Specific Patterns
- **ABP Modules**: Each layer is organized as an ABP module (e.g., `PikachuApplicationModule`, `PikachuBlazorModule`)
- **Application Services**: Business logic exposed through application services in the Application layer
- **DTOs**: Data transfer objects defined in Application.Contracts
- **Background Jobs**: Automated tasks using ABP's background job system
- **Multi-tenancy**: Built-in tenant management support
- **Localization**: Multi-language support through ABP's localization system

### Database
- **Entity Framework Core** with Code-First migrations
- **Multi-tenant** architecture support
- Custom entities extend ABP's base entity classes

### Third-Party Integrations
- **ECPay**: Payment processing (lib/Kooco.ECPay)
- **TinyMCE**: Rich text editing
- **Ant Design Blazor**: UI component library
- **Hangfire**: Background job processing

### Development Notes
- Main entry point: `src/Kooco.Pikachu.Blazor/Program.cs`
- Configuration: Uses ABP's configuration system with appsettings.json and secrets
- Logging: Serilog integration for structured logging
- Authentication: ABP Identity with OpenIddict
- Authorization: Permission-based using ABP's authorization system

When making changes, consider the DDD layering and ensure proper separation of concerns between Domain, Application, and Infrastructure layers.