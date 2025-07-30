# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

# Pikachu Blazor E-Commerce Platform

## Architecture Overview

This is a **Domain-Driven Design (DDD)** e-commerce platform built on **ABP Framework 9.0.4** with **Blazor Server** UI. The platform specializes in group buying functionality for the Taiwan market.

### Core Business Domains

- **Orders** (`src/*/Orders/`) - Complete order lifecycle management with transactions, delivery, and invoicing
- **GroupBuys** (`src/*/GroupBuys/`) - Group purchasing with special pricing and time-limited campaigns  
- **Items** (`src/*/Items/`) - Product catalog with variants, SKUs, and inventory tracking
- **Members** (`src/*/Members/`) - Customer management with tier system and shopping credits
- **Payment Integration** - ECPay, LinePay, ChinaTrust for Taiwan market
- **Logistics** - Integration with 7-11, FamilyMart, T-Cat delivery providers

### Project Structure

```
src/
├── Kooco.Pikachu.Domain.Shared/      # Shared enums, constants, DTOs
├── Kooco.Pikachu.Domain/             # Entities, aggregates, domain services
├── Kooco.Pikachu.Application.Contracts/ # Service interfaces and DTOs
├── Kooco.Pikachu.Application/        # Business logic implementation
├── Kooco.Pikachu.EntityFrameworkCore/ # Data access, repositories, migrations
├── Kooco.Pikachu.HttpApi/            # REST API controllers
├── Kooco.Pikachu.Blazor/             # Main Blazor Server UI
└── Kooco.Pikachu.DbMigrator/         # Database migration tool
```

### Key Technologies

- **.NET 9.0** with C# 12
- **Entity Framework Core 9.0** with SQL Server
- **Blazor Server** with Bootstrap 5, AntDesign, MudBlazor
- **Hangfire** for background jobs
- **Azure Blob Storage** for file management
- **OpenIddict** for authentication

## Development Commands

### Build and Run

```bash
# Build entire solution
dotnet build

# Run main Blazor application (default: https://localhost:44388)
dotnet run --project src/Kooco.Pikachu.Blazor

# Run database migrations
dotnet run --project src/Kooco.Pikachu.DbMigrator
```

### Testing

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test test/Kooco.Pikachu.Application.Tests

# Run tests matching filter
dotnet test --filter "FullyQualifiedName~OrderAppService"

# Quick test with watch mode
pwsh scripts/test-quick.ps1 -Filter "OrderAppService" -Watch

# Run tests with coverage report
pwsh scripts/run-tests-with-coverage.ps1 -OpenReport

# Update coverage badges for team
pwsh scripts/update-coverage-badges.ps1
```

### Database Operations

```bash
# Add new migration
dotnet ef migrations add MigrationName -p src/Kooco.Pikachu.EntityFrameworkCore -s src/Kooco.Pikachu.DbMigrator

# Update database
dotnet ef database update -p src/Kooco.Pikachu.EntityFrameworkCore -s src/Kooco.Pikachu.DbMigrator
```

## Configuration

### Connection Strings (appsettings.json)

- **Default**: Main application database
- **HangfireConnection**: Background job storage

### Key Configurations

- **AuthServer:Authority**: OpenIddict authentication server
- **AzureBlobStorage**: File storage configuration
- **PaymentGateways**: ECPay, LinePay settings in `PaymentGateways` table
- **CORS**: Configure allowed origins for API access

## Test Coverage Status

### Current State
- **Baseline Coverage**: 15.15% (2025-07-18)
  - Application Layer: 7.08% (needs most improvement)
  - Domain Layer: 28.02%
  - Domain.Shared: 32.14%
- **Target**: 60% in 3 months, 80% in 6 months
- **New Code Standard**: >90% coverage required

### Test Coverage Workflow

#### 1. During Development - Quick Feedback
```bash
# Run single test class (fast)
pwsh scripts/test-quick.ps1 -Filter "OrderAppService"

# Watch mode (auto-rerun on file changes)
pwsh scripts/test-quick.ps1 -Filter "OrderAppService" -Watch
```

#### 2. After Completing a Module - Check Coverage
```bash
# Run full tests and generate report
pwsh scripts/run-tests-with-coverage.ps1

# Run and auto-open report
pwsh scripts/run-tests-with-coverage.ps1 -OpenReport
```

#### 3. Update Team Shared Badges
```bash
# Update coverage badges
pwsh scripts/update-coverage-badges.ps1

# Commit updates
git add coverage/*.svg coverage/README.md
git commit -m "chore: update test coverage badges"
```

### Testing Priorities
1. **OrderAppService** - Order processing logic
2. **PaymentGatewayAppService** - Payment integration
3. **ShopCartAppService** - Shopping cart functionality
4. **InventoryAppService** - Stock management

### Test-Related Files
- Test Plan: `docs/test-coverage-improvement-plan.md`
- Coverage Report: `coverage/reports/index.html`
- Coverage Summary: `coverage/README.md`

## Important Architectural Patterns

### Domain Aggregates

Each aggregate root has its own folder with:
- Entity classes
- Repository interface 
- Domain service (if needed)
- Specifications

Example: `Order` aggregate includes `Order`, `OrderItem`, `OrderTransaction`, `OrderDelivery`

### Application Services

Follow pattern:
1. Input validation via DTOs
2. Permission checks
3. Domain logic execution
4. Return DTOs (not entities)

### Repository Pattern

- Use `IRepository<TEntity>` from ABP
- Custom repositories inherit from `EfCoreRepository`
- Complex queries use specifications

### Background Jobs

Hangfire jobs located in `BackgroundJobs/` folders:
- Order completion checks
- Inventory updates
- Email notifications

## Common Development Tasks

### Adding a New Feature

1. Define domain entities in `Domain` project
2. Create DTOs in `Application.Contracts`
3. Implement service in `Application`
4. Add API controller in `HttpApi`
5. Create Blazor UI components
6. Write tests in corresponding test project

### Working with Orders

Orders have complex state management:
- Status: Pending → Processing → Shipped → Completed
- Payment states tracked separately
- Inventory deduction on confirmation
- Integration with logistics providers

### Payment Gateway Integration

Payment flow:
1. Create order with pending payment
2. Redirect to payment gateway
3. Handle callback in `OrderAppService`
4. Update order and inventory on success

## Known Issues and Warnings

- **ECPay.Payment.Integration**: Shows NU1701 warning (targets .NET Framework) but works in .NET 9
- **SQLite In-Memory**: Test database resets between test runs (location: `:memory:`)
- **Blob Storage**: Requires Azure connection in production
- **Test Database**: Automatically rebuilds for each test run

## Project-Specific Conventions

- UI uses mix of Bootstrap, AntDesign, and MudBlazor (transitioning to unified approach)
- Background jobs use Hangfire with dedicated connection string
- Multi-tenancy enabled but currently single-tenant deployment
- Localization files in `Localization/Pikachu/` support zh-Hant, zh-Hans, en

## Testing Coverage Status

### Current Coverage (Baseline: 2025-01-18)
- Overall: 15.15%
- Application Layer: 7.08%
- Domain Layer: 33.75%
- EntityFrameworkCore Layer: 17.89%
- HttpApi Layer: 0%

### Completed Tests
- ✅ **OrderAppService** - Full test suite implemented
  - OrderAppServicePaymentTests.cs
  - OrderAppServiceStatusTests.cs
  - OrderAppServiceQueryTests.cs
  - OrderAppServiceBusinessLogicTests.cs
  - TestOrderDataBuilder.cs (Test data builder helper)

### Next Priority Tests
1. **PaymentGatewayAppService** - Payment integration (HIGH)
2. **ShopCartAppService** - Shopping cart (HIGH)
3. **GroupBuyAppService** - Group buying features (HIGH)
4. **ItemAppService** - Product management (MEDIUM)
5. **InventoryAppService** - Inventory management (MEDIUM)

### Testing Infrastructure
- Test data builder pattern established with `TestOrderDataBuilder`
- SQLite in-memory database with RowVersion constraint handling
- Coverage tools: Coverlet + ReportGenerator
- Test scripts in `scripts/` folder for quick execution

## Current Work Focus

1. ✅ Write tests for OrderAppService (COMPLETED)
2. Next: Implement PaymentGatewayAppService tests
3. Target: Reach 40% Application layer coverage

## Database Documentation Commands

- Use `db2dbml` to generate DBML files for database reference:
  * Command: `db2dbml mssql 'Server=tcp:dev-kdbs.database.windows.net,1433;Initial Catalog=noodle-db-dev;Persist Security Info=False;User ID=kooco_admin;Password=Koo2915co;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;' -o docs/database.dbml`
  * Purpose: Generate database schema documentation in DBML format
  * Save location: `docs/` folder for easy reference
```

Before Starting Any Task using noodle-memory for following:
Always search first: Use the search_nodes tool to look for relevant preferences and procedures before beginning work.
Search for facts too: Use the search_facts tool to discover relationships and factual information that may be relevant to your task.
Filter by entity type: Specify Preference, Procedure, or Requirement in your node search to get targeted results.
Review all matches: Carefully examine any preferences, procedures, or facts that match your current task.
Always Save New or Updated Information
Capture requirements and preferences immediately: When a user expresses a requirement or preference, use add_memory to store it right away.
Best practice: Split very long requirements into shorter, logical chunks.
Be explicit if something is an update to existing knowledge. Only add what's changed or new to the graph.
Document procedures clearly: When you discover how a user wants things done, record it as a procedure.
Record factual relationships: When you learn about connections between entities, store these as facts.
Be specific with categories: Label preferences and procedures with clear categories for better retrieval later.
During Your Work
Respect discovered preferences: Align your work with any preferences you've found.
Follow procedures exactly: If you find a procedure for your current task, follow it step by step.
Apply relevant facts: Use factual information to inform your decisions and recommendations.
Stay consistent: Maintain consistency with previously identified preferences, procedures, and facts.
Best Practices
Search before suggesting: Always check if there's established knowledge before making recommendations.
Combine node and fact searches: For complex tasks, search both nodes and facts to build a complete picture.
Use center_node_uuid: When exploring related information, center your search around a specific node.
Prioritize specific matches: More specific information takes precedence over general information.
Be proactive: If you notice patterns in user behavior, consider storing them as preferences or procedures.
Remember: The knowledge graph is your memory. Use it consistently to provide personalized assistance that respects the user's established preferences, procedures, and factual context.

Always use traditional chinese for doc and memory

## Jira Task Point Guidance

- 以後在 Jira 建立 task 的時候，如果需要輸入 story point 時請依照這個任務的需要花費一位資深工程師的時間來設置