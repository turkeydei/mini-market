$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}

Write-Host "=== MINI MARKET - SETUP & RUN SCRIPT ==="

# Step 1: Check .NET SDK
Write-Host "Step 1: Checking .NET SDK..."
try {
    $dotnetVersion = & dotnet --version
    Write-Host "OK .NET SDK installed (Version: $dotnetVersion)"
} catch {
    Write-Host "ERROR .NET SDK is not installed!"
    Write-Host "Download: https://dotnet.microsoft.com/download"
    exit 1
}

# Step 2: Check EF Core Tools
Write-Host ""
Write-Host "Step 2: Checking EF Core Tools..."
$efOk = $true
try { & dotnet ef --version | Out-Null } catch { $efOk = $false }
if (-not $efOk) {
    Write-Host "EF Core Tools not found. Installing..."
    & dotnet tool install --global dotnet-ef
    $env:PATH = "$($env:USERPROFILE)\.dotnet\tools;$env:PATH"
    Write-Host "EF Core Tools installed."
} else {
    Write-Host "OK EF Core Tools already installed."
}

# Step 3: Restore
Write-Host ""
Write-Host "Step 3: Restoring dependencies..."
& dotnet restore
Write-Host "Dependencies restored."

# Step 4: Remove old migrations (if any)
Write-Host ""
Write-Host "Step 4: Removing old migrations (if any)..."
Push-Location
Set-Location "Persistence"
if (Test-Path "Migrations") {
    Remove-Item -Recurse -Force "Migrations"
    Write-Host "Old migrations removed."
} else {
    Write-Host "No existing migrations."
}

# Step 5: Create migration
Write-Host ""
Write-Host "Step 5: Creating new migration..."
& dotnet ef migrations add SimplifiedSchema --startup-project "..\WebShop"
Write-Host "Migration created."

# Step 6: Check Docker and SQL Server
Write-Host ""
Write-Host "Step 6: Checking SQL Server..."
$dockerInstalled = $false
try { 
    $dockerVersion = & docker --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        $dockerInstalled = $true
        Write-Host "OK Docker is installed."
        
        # Check if SQL Server container is running
        $sqlContainer = & docker ps --filter "name=sqlserver" --format "{{.Names}}" 2>&1
        if ($sqlContainer -eq "sqlserver") {
            Write-Host "OK SQL Server container is running."
        } else {
            Write-Host "WARNING SQL Server container not found. Please start it manually:"
            Write-Host "  docker run -e `"ACCEPT_EULA=Y`" -e `"SA_PASSWORD=Admin@123456`" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest"
            Write-Host ""
            Write-Host "Or use Docker Compose to run everything:"
            Write-Host "  docker-compose up -d"
        }
    }
} catch {
    Write-Host "WARNING Docker not found. Make sure SQL Server is running on localhost,1433"
}

Write-Host ""
Write-Host "Connection String: Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True;Encrypt=False"
Write-Host ""
Read-Host "Press Enter to update database (Ctrl+C to cancel)"

# Step 7: Update database
Write-Host ""
Write-Host "Step 7: Updating database..."
& dotnet ef database update --startup-project "..\WebShop"
Write-Host "Database updated."
Pop-Location

# Step 8: Build project
Write-Host ""
Write-Host "Step 8: Building project..."
Push-Location
Set-Location "WebShop"
& dotnet build
Write-Host "Build succeeded."
Pop-Location

Write-Host ""
Write-Host "=== SETUP DONE ==="
Write-Host ""
Write-Host "Option 1: Run with Docker (Recommended):"
Write-Host "  docker-compose up -d"
Write-Host "  Then open: http://localhost:5000"
Write-Host ""
Write-Host "Option 2: Run locally:"
Write-Host "  cd WebShop"
Write-Host "  dotnet run"
Write-Host "  Then open: http://localhost:5000"
Write-Host ""
Write-Host "Demo accounts:"
Write-Host "  Admin: admin / Admin@123"
Write-Host "  User:  user1 / User@123"
