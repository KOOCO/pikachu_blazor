# Extract Domain Shared APIs Script Usage

The `extract_domain_shared_apis.ps1` script has been refactored to accept build output dynamically, improving reusability and maintainability.

## Usage Examples

### 1. Build project and extract APIs (default)
```powershell
./extract_domain_shared_apis.ps1
```

### 2. Specify a different project path
```powershell
./extract_domain_shared_apis.ps1 -ProjectPath "src/Kooco.Pikachu.Application.Contracts"
```

### 3. Pass build output directly
```powershell
$buildOutput = dotnet build src/Kooco.Pikachu.Domain.Shared 2>&1 | Out-String
./extract_domain_shared_apis.ps1 -BuildOutput $buildOutput
```

### 4. Read build output from file
```powershell
# Save build output to file first
dotnet build src/Kooco.Pikachu.Domain.Shared 2>&1 > build_output.txt

# Then extract APIs from the file
./extract_domain_shared_apis.ps1 -FromFile -InputFile build_output.txt
```

### 5. Save extracted APIs to file
```powershell
./extract_domain_shared_apis.ps1 -OutputFile "missing_domain_apis.txt"
```

### 6. Complete pipeline example
```powershell
# Build and save output
dotnet build src/Kooco.Pikachu.Domain.Shared --verbosity minimal 2>&1 > domain_build.txt

# Extract APIs and save to file
./extract_domain_shared_apis.ps1 -FromFile -InputFile domain_build.txt -OutputFile "domain_apis.txt"

# Use the extracted APIs
Get-Content domain_apis.txt | Add-Content src/Kooco.Pikachu.Domain.Shared/PublicAPI.Unshipped.txt
```

## Parameters

- **ProjectPath**: Path to the project to build (default: "src/Kooco.Pikachu.Domain.Shared")
- **BuildOutput**: Build output string to process
- **OutputFile**: File path to save extracted API signatures
- **FromFile**: Switch to read build output from a file
- **InputFile**: Path to file containing build output (used with -FromFile)

## Return Value

The script returns the count of missing API signatures found (exit code).