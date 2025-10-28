# 🛒 Mini Market - ASP.NET Core MVC

Modern e-commerce web application built with **3-tier architecture**, **Repository Pattern**, and **Unit of Work Pattern**.

## 📸 Features

- 🔐 **Authentication & Authorization** - Cookie-based authentication
- 🛍️ **Shopping Cart** - LocalStorage-based cart
- 📦 **Product Management** - Categories, products with images
- 💳 **Checkout Flow** - Order creation with payment tracking
- 📊 **Order Management** - Order history, details, status tracking
- 👤 **User Profile** - Registration, login, profile management

## 🏗️ Architecture

```
┌────────────────────┐
│  WebShop (MVC)     │  ← Presentation Layer
├────────────────────┤
│  Application       │  ← Business Logic (Services)
├────────────────────┤
│  Persistence       │  ← Data Access (Repository, UnitOfWork)
├────────────────────┤
│  Domain            │  ← Entities (User, Product, Order)
└────────────────────┘
```

**Patterns Implemented:**
- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Service Layer Pattern
- ✅ Dependency Injection

📖 **[View Full Architecture Documentation](ARCHITECTURE.md)**

## 🚀 Quick Start

### Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for SQL Server)
- Git

---

## 💻 Setup for Windows Developers

### 1️⃣ Install Prerequisites

#### Install .NET 9.0 SDK
1. Download from: https://dotnet.microsoft.com/download/dotnet/9.0
2. Run installer `dotnet-sdk-9.0.xxx-win-x64.exe`
3. Verify installation:
```powershell
dotnet --version
# Should show: 9.0.x
```

#### Install Docker Desktop
1. Download from: https://www.docker.com/products/docker-desktop/
2. Install and restart Windows
3. Start Docker Desktop
4. Verify installation:
```powershell
docker --version
# Should show: Docker version xx.x.x
```

#### Install Git (if not installed)
1. Download from: https://git-scm.com/download/win
2. Use default settings during installation

#### Install EF Core Tools
```powershell
dotnet tool install --global dotnet-ef
```

### 2️⃣ Clone & Setup Project

**Open PowerShell or Windows Terminal:**

```powershell
# Clone repository
git clone https://github.com/turkeydei/mini-market.git
cd mini-market

# Run setup script (automated)
.\setup.ps1
```

### 3️⃣ Start SQL Server (Docker)

```powershell
# Start SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# Verify container is running
docker ps
```

### 4️⃣ Apply Database Migrations

```powershell
# Navigate to WebShop project
cd WebShop

# Apply migrations
dotnet ef database update

# Verify database created successfully
```

### 5️⃣ Run Application

```powershell
# Run the application
dotnet run --urls "http://localhost:5000"

# Or use the run script
cd ..
.\run.ps1
```

### 6️⃣ Open Browser

Navigate to: **http://localhost:5000**

---

## 🍎 Setup for macOS/Linux

### Quick Setup Script

```bash
# Clone repository
git clone https://github.com/turkeydei/mini-market.git
cd mini-market

# Run automated setup
./quick-start.sh
```

### Manual Setup

```bash
# 1. Start SQL Server (Docker)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# 2. Apply database migrations
cd WebShop
dotnet ef database update

# 3. Run application
dotnet run --urls "http://localhost:5000"
```

**Open browser:** http://localhost:5000

## 🔑 Demo Accounts

| Role | Username | Password | Email |
|------|----------|----------|-------|
| Admin | `admin` | `Admin@123` | admin@minimarket.com |
| User | `user1` | `User@123` | user1@minimarket.com |

## 📦 Tech Stack

- **Backend:** ASP.NET Core MVC 9.0
- **ORM:** Entity Framework Core
- **Database:** SQL Server 2022
- **Frontend:** Bootstrap 5 + jQuery
- **Icons:** Bootstrap Icons
- **Authentication:** Cookie-based Auth

## 📁 Project Structure

```
mini-market/
├── Domain/              # Entities (User, HangHoa, HoaDon, etc.)
├── Application/         # Business Logic (Services, Interfaces)
├── Persistence/         # Data Access (DbContext, Repositories)
└── WebShop/            # Web Application (Controllers, Views)
    ├── Controllers/    # MVC Controllers
    ├── Views/         # Razor Views
    ├── Models/        # ViewModels
    └── wwwroot/       # Static files (CSS, JS, images)
```

## 📖 Key Entities

- **User** - User accounts with roles (Admin/User)
- **Loai** - Product categories
- **HangHoa** - Products with pricing and stock
- **HoaDon** - Orders with status tracking
- **ChiTietHD** - Order line items
- **PaymentTransaction** - Payment records

## 🛠️ Development

### Database Migrations

```bash
# Create new migration
dotnet ef migrations add MigrationName -p ../Persistence -s WebShop

# Apply migrations
dotnet ef database update -p ../Persistence -s WebShop

# Remove last migration
dotnet ef migrations remove -p ../Persistence -s WebShop
```

### Build & Test

```bash
# Build solution
dotnet build

# Run tests
dotnet test

# Clean build
dotnet clean
```

## 🐛 Troubleshooting

### Database Connection Failed
**Solution:**
- Ensure Docker Desktop is running
- Check SQL Server container: `docker ps`
- Restart container: `docker restart sqlserver`
- Check connection string in `appsettings.json`

### Port Already in Use

**Windows (PowerShell):**
```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID with actual process ID)
taskkill /PID <PID> /F
```

**macOS/Linux:**
```bash
# Find process using port 5000
lsof -i :5000

# Kill process
kill -9 <PID>
```

### EF Core Tools Not Found
```bash
# Install EF Core tools globally
dotnet tool install --global dotnet-ef

# Update if already installed
dotnet tool update --global dotnet-ef

# Verify installation
dotnet ef --version
```

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Docker Container Won't Start (Windows)

**Check WSL 2:**
```powershell
wsl --list --verbose
# Should show WSL 2 installed
```

**Enable Virtualization:**
1. Restart PC → Enter BIOS (F2/Del)
2. Enable "Intel VT-x" or "AMD-V"
3. Save and restart

**Docker Desktop Issues:**
- Restart Docker Desktop
- Check Docker settings → Resources → ensure WSL 2 is enabled
- Try: `docker system prune -a` (removes all containers/images)

### SQL Server Container Exits Immediately

**Check logs:**
```powershell
docker logs sqlserver
```

**Common fixes:**
- Increase Docker memory: Docker Desktop → Settings → Resources → Memory (min 4GB)
- Remove old container: `docker rm -f sqlserver`
- Recreate container with correct password

### Cannot Access http://localhost:5000

**Check if app is running:**
```powershell
# Windows
netstat -ano | findstr :5000

# macOS/Linux  
lsof -i :5000
```

**Firewall issues (Windows):**
- Add exception for port 5000 in Windows Defender Firewall
- Or run PowerShell as Administrator

### Visual Studio Issues

**If using Visual Studio 2022:**
1. Open `MiniMarket.sln`
2. Set `WebShop` as Startup Project
3. Update `launchSettings.json` if needed
4. Run with IIS Express or Kestrel

### Database Migration Errors

**"No migrations found":**
```bash
cd WebShop
dotnet ef migrations add InitialCreate -p ../Persistence
dotnet ef database update
```

**"Pending model changes":**
```bash
# Create new migration for changes
dotnet ef migrations add YourMigrationName -p ../Persistence

# Apply migration
dotnet ef database update
```

## 📝 API Endpoints

### Public
- `GET /` - Home page with products
- `GET /Auth/Login` - Login page
- `GET /Auth/Register` - Registration page

### Authenticated
- `GET /Checkout` - Checkout page
- `POST /Checkout/CreateOrder` - Create order
- `GET /Order/History` - Order history
- `GET /Order/Details/{id}` - Order details
- `GET /Auth/Profile` - User profile

## 🤝 Contributing

1. Fork the repository
2. Create feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to branch (`git push origin feature/AmazingFeature`)
5. Open Pull Request

## 📄 License

This project is licensed under the MIT License.

## 👨‍💻 Author

**turkeydei** - [GitHub](https://github.com/turkeydei)

---

**⭐ Star this repo if you find it helpful!**
