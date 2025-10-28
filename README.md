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

### Setup & Run

```bash
# 1. Clone repository
git clone https://github.com/turkeydei/mini-market.git
cd mini-market

# 2. Start SQL Server (Docker)
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# 3. Apply database migrations
cd WebShop
dotnet ef database update

# 4. Run application
dotnet run --urls "http://localhost:5000"
```

**Open browser:** http://localhost:5000

### Quick Setup Scripts

**macOS/Linux:**
```bash
./quick-start.sh
```

**Windows (PowerShell):**
```powershell
.\setup.ps1
.\run.ps1
```

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
- Ensure Docker is running
- Check SQL Server container: `docker ps`
- Restart container: `docker restart sqlserver`

### Port Already in Use
```bash
# Find process using port 5000
lsof -i :5000

# Kill process
kill -9 <PID>
```

### EF Core Tools Not Found
```bash
# Install EF Core tools
dotnet tool install --global dotnet-ef
```

### Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
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
