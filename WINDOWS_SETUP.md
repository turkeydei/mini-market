# 🪟 Windows Setup Guide for Mini Market

Complete guide for Windows developers to setup and run this ASP.NET Core project.

## 📋 Table of Contents

1. [Prerequisites Installation](#prerequisites-installation)
2. [Project Setup](#project-setup)
3. [Database Setup](#database-setup)
4. [Running the Application](#running-the-application)
5. [Troubleshooting](#troubleshooting)
6. [IDE Setup](#ide-setup)

---

## 🔧 Prerequisites Installation

### 1. Install .NET 9.0 SDK

**Download & Install:**
1. Visit: https://dotnet.microsoft.com/download/dotnet/9.0
2. Download **Windows x64 Installer**
3. Run `dotnet-sdk-9.0.xxx-win-x64.exe`
4. Follow installation wizard

**Verify Installation:**
```powershell
# Open PowerShell or Command Prompt
dotnet --version
# Expected output: 9.0.x

dotnet --list-sdks
# Should show .NET 9.0 SDK
```

### 2. Install Docker Desktop

**Download & Install:**
1. Visit: https://www.docker.com/products/docker-desktop/
2. Download **Docker Desktop for Windows**
3. Run installer
4. **Important:** Enable WSL 2 during installation
5. Restart computer after installation

**Verify Installation:**
```powershell
docker --version
# Expected: Docker version xx.x.x

docker ps
# Should show: "CONTAINER ID  IMAGE  ..."
```

**Enable WSL 2 (if not enabled):**
```powershell
# Check WSL version
wsl --list --verbose

# Install WSL 2
wsl --install

# Set WSL 2 as default
wsl --set-default-version 2
```

### 3. Install Git for Windows

**Download & Install:**
1. Visit: https://git-scm.com/download/win
2. Download and run installer
3. Use **default settings** during installation
4. Enable "Git Bash Here" context menu option

**Verify Installation:**
```powershell
git --version
# Expected: git version x.xx.x
```

### 4. Install Entity Framework Core Tools

```powershell
# Install globally
dotnet tool install --global dotnet-ef

# Verify installation
dotnet ef --version
# Expected: Entity Framework Core .NET Command-line Tools x.x.x
```

---

## 🚀 Project Setup

### Method 1: Using PowerShell Script (Recommended)

**Open PowerShell:**
```powershell
# Clone repository
git clone https://github.com/turkeydei/mini-market.git
cd mini-market

# Run automated setup script
.\setup.ps1

# The script will:
# - Check prerequisites
# - Start SQL Server container
# - Apply database migrations
# - Setup demo data
```

### Method 2: Manual Setup

**Step-by-step commands:**

```powershell
# 1. Clone repository
git clone https://github.com/turkeydei/mini-market.git
cd mini-market

# 2. Restore NuGet packages
dotnet restore

# 3. Build solution
dotnet build

# If build successful, proceed to database setup
```

---

## 💾 Database Setup

### 1. Start SQL Server Container

```powershell
# Start SQL Server in Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# Verify container is running
docker ps

# Expected output:
# CONTAINER ID   IMAGE                            STATUS
# xxxxx          mcr.microsoft.com/mssql/server   Up xx seconds
```

### 2. Apply Database Migrations

```powershell
# Navigate to WebShop project
cd WebShop

# Apply migrations (creates database + tables + seed data)
dotnet ef database update

# Expected output:
# Applying migration '20251028031220_InitialCreate'
# Done.
```

### 3. Verify Database

```powershell
# Check SQL Server logs
docker logs sqlserver

# Or connect with SQL Server Management Studio (SSMS)
# Server: localhost,1433
# Login: sa
# Password: Admin@123456
```

---

## ▶️ Running the Application

### Method 1: Using PowerShell Script

```powershell
# From project root
.\run.ps1

# Opens browser automatically at http://localhost:5000
```

### Method 2: Manual Start

```powershell
# From project root
cd WebShop

# Run application
dotnet run --urls "http://localhost:5000"

# Expected output:
# Now listening on: http://localhost:5000
# Application started. Press Ctrl+C to shut down.
```

### Method 3: Visual Studio 2022

1. Open `MiniMarket.sln` in Visual Studio
2. Right-click `WebShop` → **Set as Startup Project**
3. Press **F5** or click **Start Debugging**
4. Choose **Kestrel** profile (not IIS Express)

### Access the Application

Open browser and navigate to:
- **http://localhost:5000**

Default accounts:
- Admin: `admin` / `Admin@123`
- User: `user1` / `User@123`

---

## 🐛 Troubleshooting

### ❌ "dotnet command not found"

**Solution:**
1. Re-install .NET SDK
2. Add to PATH manually:
   - Search "Environment Variables" in Windows
   - Add `C:\Program Files\dotnet\` to System PATH
3. Restart PowerShell/CMD

### ❌ Docker won't start

**Check Virtualization:**
```powershell
# Run as Administrator
Get-ComputerInfo | Select-Object -Property HyperVisorPresent, HyperVRequirementVirtualizationFirmwareEnabled
```

**If False:**
1. Restart PC → Enter BIOS (usually F2, F10, or Del key)
2. Find "Virtualization Technology" or "Intel VT-x" / "AMD-V"
3. Enable it
4. Save and restart

**Docker Desktop not starting:**
- Ensure WSL 2 is installed: `wsl --install`
- Restart Docker Desktop
- Reset to factory defaults: Docker → Settings → Troubleshoot → Reset

### ❌ SQL Server container exits immediately

**Check logs:**
```powershell
docker logs sqlserver
```

**Common issues:**

1. **Insufficient memory:**
   - Docker Desktop → Settings → Resources
   - Set Memory to at least **4 GB**

2. **Container already exists:**
```powershell
# Remove old container
docker rm -f sqlserver

# Recreate
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

3. **Password too weak:**
   - Must contain uppercase, lowercase, digits, and special chars
   - Minimum 8 characters

### ❌ Port 5000 already in use

**Find process using port:**
```powershell
netstat -ano | findstr :5000
# Output: TCP  0.0.0.0:5000  0.0.0.0:0  LISTENING  12345
#                                                   ↑ PID
```

**Kill process:**
```powershell
taskkill /PID 12345 /F
```

### ❌ Database connection failed

**Check connection string:**
```json
// WebShop/appsettings.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True"
  }
}
```

**Verify SQL Server is running:**
```powershell
docker ps
# Should show sqlserver container
```

**Test connection:**
```powershell
# Using sqlcmd (if installed)
sqlcmd -S localhost,1433 -U sa -P Admin@123456 -Q "SELECT @@VERSION"
```

### ❌ EF Core migration errors

**"No migrations found":**
```powershell
cd WebShop
dotnet ef migrations add InitialCreate -p ../Persistence
dotnet ef database update
```

**"Pending model changes":**
```powershell
# Create new migration
dotnet ef migrations add YourMigrationName -p ../Persistence -s WebShop

# Apply
dotnet ef database update -p ../Persistence -s WebShop
```

**"Build failed":**
```powershell
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### ❌ Firewall blocking port 5000

**Add firewall rule:**
```powershell
# Run as Administrator
New-NetFirewallRule -DisplayName "ASP.NET Core Port 5000" -Direction Inbound -LocalPort 5000 -Protocol TCP -Action Allow
```

**Or disable firewall temporarily:**
- Windows Defender Firewall → Turn off (not recommended for production)

---

## 💻 IDE Setup

### Visual Studio 2022

**Recommended Extensions:**
- Web Live Preview
- GitHub Copilot (optional)
- ReSharper (optional)

**Setup Steps:**
1. Open `MiniMarket.sln`
2. Right-click **WebShop** → Set as Startup Project
3. Press **Ctrl+F5** to run without debugging
4. Or **F5** to run with debugging

**Debug Configuration:**
- Use **Kestrel** profile (faster)
- Or **IIS Express** (if you prefer)

### Visual Studio Code

**Required Extensions:**
- C# Dev Kit
- .NET Install Tool
- Docker
- GitLens (optional)

**Setup:**
1. Open folder `mini-market`
2. Install recommended extensions (VSCode will prompt)
3. Press **F5** to start debugging
4. Or use terminal: `dotnet run`

**Create launch.json:**
```json
{
  "version": "0.2.0",
  "configurations": [
    {
      "name": ".NET Core Launch (web)",
      "type": "coreclr",
      "request": "launch",
      "preLaunchTask": "build",
      "program": "${workspaceFolder}/WebShop/bin/Debug/net9.0/WebShop.dll",
      "args": [],
      "cwd": "${workspaceFolder}/WebShop",
      "stopAtEntry": false,
      "serverReadyAction": {
        "action": "openExternally",
        "pattern": "\\bNow listening on:\\s+(https?://\\S+)"
      },
      "env": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    }
  ]
}
```

### JetBrains Rider

**Setup:**
1. Open `MiniMarket.sln`
2. Rider will auto-detect run configurations
3. Select **WebShop** configuration
4. Click Run (Shift+F10) or Debug (Shift+F9)

---

## 🔄 Daily Workflow

### Starting Development

```powershell
# 1. Start Docker Desktop (if not running)

# 2. Start SQL Server container (if stopped)
docker start sqlserver

# 3. Navigate to project
cd C:\path\to\mini-market\WebShop

# 4. Run application
dotnet run --urls "http://localhost:5000"
```

### Stopping Development

```powershell
# 1. Stop application (Ctrl+C in terminal)

# 2. (Optional) Stop SQL Server container
docker stop sqlserver

# Keep container running if you'll develop again soon
```

### Updating Code from GitHub

```powershell
# Pull latest changes
git pull origin main

# Restore packages (if needed)
dotnet restore

# Apply new migrations (if any)
cd WebShop
dotnet ef database update

# Run
dotnet run
```

---

## 📚 Additional Resources

### Official Documentation
- [ASP.NET Core](https://docs.microsoft.com/aspnet/core)
- [Entity Framework Core](https://docs.microsoft.com/ef/core)
- [Docker for Windows](https://docs.docker.com/desktop/windows/)

### Useful Commands

```powershell
# Clean everything
dotnet clean
docker system prune -a

# View Docker logs
docker logs sqlserver -f

# Database backup (optional)
docker exec sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin@123456 -Q "BACKUP DATABASE MiniMarketDB TO DISK='/tmp/backup.bak'"

# List all Docker containers
docker ps -a

# Remove all stopped containers
docker container prune
```

---

## 🎯 Quick Reference

| Task | Command |
|------|---------|
| Build project | `dotnet build` |
| Run application | `dotnet run` |
| Start SQL Server | `docker start sqlserver` |
| Stop SQL Server | `docker stop sqlserver` |
| Apply migrations | `dotnet ef database update` |
| Create migration | `dotnet ef migrations add Name -p ../Persistence` |
| View containers | `docker ps` |
| View logs | `docker logs sqlserver` |

---

## ❓ Need Help?

1. Check [ARCHITECTURE.md](ARCHITECTURE.md) for project structure
2. Check [README.md](README.md) for general documentation
3. Open an issue on GitHub
4. Contact: **turkeydei** on GitHub

---

**Happy Coding! 🚀**

