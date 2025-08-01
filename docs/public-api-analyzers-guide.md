# PublicApiAnalyzers Integration Guide

## Overview

Microsoft.CodeAnalysis.PublicApiAnalyzers has been integrated into the Pikachu Blazor project to prevent accidental breaking changes in public APIs. This guide explains how to work with the analyzers and manage public API declarations.

## What are PublicApiAnalyzers?

PublicApiAnalyzers help track and manage public API surface by:
- Detecting when public APIs are added, removed, or changed
- Enforcing that all public APIs are explicitly declared
- Preventing accidental breaking changes
- Providing a clear history of API evolution

## Enabled Projects

The analyzers are enabled for these projects:
- `Kooco.Pikachu.Application.Contracts` - Service interfaces and DTOs
- `Kooco.Pikachu.HttpApi` - Web API controllers  
- `Kooco.Pikachu.Domain.Shared` - Shared types and constants

## Key Files

Each enabled project contains:
- `PublicAPI.Shipped.txt` - APIs that have been released/shipped
- `PublicAPI.Unshipped.txt` - New APIs pending release

## Common Scenarios

### Adding a New Public API

When you add a new public type, method, or property, you'll get an RS0016 warning:

```
warning RS0016: Symbol 'MyNewClass' is not part of the declared public API
```

**Solution**: Add the API signature to `PublicAPI.Unshipped.txt`

### Removing a Public API

When you remove a public API, you'll get an RS0017 warning:

```
warning RS0017: Symbol 'OldMethod' is part of the declared API but cannot be found
```

**Solution**: Remove the API signature from the PublicAPI files

## Using the Scripts

### Auto-update Script (Recommended)

```bash
# Automatically fix all API issues
pwsh scripts/api-protection/auto-update-public-api.ps1

# Check only (for CI/CD)
pwsh scripts/api-protection/auto-update-public-api.ps1 -CheckOnly
```

### Extract Missing APIs

```bash
# Extract missing APIs from a specific project
pwsh extract_missing_apis.ps1 -ProjectPath "src/Kooco.Pikachu.HttpApi"
```

### Manual Update

```bash
# Quick local update
pwsh scripts/update-api.sh
```

## CI/CD Integration

The Azure DevOps pipeline automatically:
1. Runs auto-update to fix any missing APIs
2. Checks for uncommitted changes
3. Validates all APIs are properly declared

If the build fails with API errors:
1. Run `pwsh scripts/api-protection/auto-update-public-api.ps1` locally
2. Commit the updated PublicAPI files
3. Push your changes

## Best Practices

### 1. Review API Changes

Always review changes to PublicAPI files before committing:
```bash
git diff **/PublicAPI*.txt
```

### 2. Document Breaking Changes

If you must make a breaking change:
1. Document it in release notes
2. Consider the impact on consumers
3. Follow semantic versioning

### 3. Ship APIs on Release

When releasing a new version:
1. Move entries from `PublicAPI.Unshipped.txt` to `PublicAPI.Shipped.txt`
2. Clear the Unshipped file
3. Tag the release

### 4. Keep APIs Minimal

Only expose what's necessary:
- Use `internal` instead of `public` when possible
- Consider using interfaces to hide implementation details
- Group related APIs in namespaces

## Troubleshooting

### Build Fails with RS0016/RS0017

Run the auto-update script:
```bash
pwsh scripts/api-protection/auto-update-public-api.ps1
```

### PowerShell Script Errors

Ensure you have PowerShell Core installed:
```bash
# Windows
winget install Microsoft.PowerShell

# macOS
brew install powershell

# Linux
snap install powershell --classic
```

### Duplicate API Entries

Clean up duplicates:
```bash
sort PublicAPI.Unshipped.txt | uniq > temp.txt
mv temp.txt PublicAPI.Unshipped.txt
```

### CI/CD Pipeline Failures

Check the build log for specific errors. Common issues:
- Uncommitted PublicAPI changes
- PowerShell version incompatibility
- Missing API declarations

## Configuration

The analyzers are configured in `common.props`:

```xml
<PropertyGroup Condition="'$(EnableApiAnalyzers)' == 'true'">
  <TreatWarningsAsErrors Condition="'$(CI)' == 'true'">true</TreatWarningsAsErrors>
  <WarningsAsErrors Condition="'$(CI)' == 'true'">RS0016;RS0017</WarningsAsErrors>
</PropertyGroup>
```

To temporarily disable:
```bash
dotnet build -p:EnableApiAnalyzers=false
```

## API Signature Format

Common API signature patterns:

```
# Class
Kooco.Pikachu.Orders.OrderDto

# Constructor
Kooco.Pikachu.Orders.OrderDto.OrderDto() -> void

# Property
Kooco.Pikachu.Orders.OrderDto.Id.get -> System.Guid
Kooco.Pikachu.Orders.OrderDto.Id.set -> void

# Method
Kooco.Pikachu.Orders.IOrderAppService.GetAsync(System.Guid id) -> System.Threading.Tasks.Task<Kooco.Pikachu.Orders.OrderDto!>!

# Constant
const Kooco.Pikachu.OrderConsts.MaxNameLength = 100 -> int
```

## Further Reading

- [Microsoft Docs: PublicApiAnalyzers](https://github.com/dotnet/roslyn-analyzers/blob/main/src/PublicApiAnalyzers/PublicApiAnalyzers.Help.md)
- [Preventing Breaking Changes in .NET Libraries](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/breaking-changes)
- [Semantic Versioning](https://semver.org/)

## Support

For questions or issues:
1. Check this guide first
2. Run the troubleshooting steps
3. Ask the team lead or DevOps engineer
4. Create a GitHub issue if needed