# 🔧 TROUBLESHOOTING - Giải quyết lỗi thường gặp

## ❌ Lỗi: "This site can't be reached" / "Connection refused"

### Nguyên nhân

- Ứng dụng chưa chạy hoặc đã bị crash
- Database connection bị lỗi
- Migration chưa được chạy

### Giải pháp

#### Bước 1: Kiểm tra xem app có đang chạy không

```bash
# Mở terminal mới và kiểm tra
ps aux | grep dotnet
# hoặc
lsof -i :5000
lsof -i :5001
```

Nếu không thấy process nào → App không chạy

#### Bước 2: Chạy app với verbose để xem lỗi chi tiết

```bash
cd WebShop
dotnet run --verbosity detailed
```

**Chú ý xem output:**

- Nếu thấy lỗi về SQL Server → Xem phần "SQL Server Connection" bên dưới
- Nếu thấy lỗi về Migrations → Chạy migrations thủ công
- Nếu thấy "Port already in use" → Đổi port hoặc kill process đang dùng

#### Bước 3: Port đã được cập nhật

Tôi đã sửa port thành:

- **HTTP**: http://localhost:5000
- **HTTPS**: https://localhost:5001

Thử lại sau khi chạy `dotnet run`.

---

## ❌ Lỗi: HTTP ERROR 403 (Access Denied)

### Nguyên nhân

1. SSL Certificate chưa được trust
2. App đang chạy nhưng có lỗi routing
3. Đang truy cập sai URL

### Giải pháp

#### Option 1: Sử dụng HTTP thay vì HTTPS

```bash
cd WebShop
dotnet run --urls "http://localhost:5000"
```

Sau đó truy cập: **http://localhost:5000** (không dùng HTTPS)

#### Option 2: Trust SSL Certificate (Cho HTTPS)

```bash
# macOS/Linux
dotnet dev-certs https --trust

# Nếu lỗi, xóa và tạo lại
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

#### Option 3: Disable HTTPS Redirection (Temporary)

Mở `Program.cs` và comment dòng này:

```csharp
// app.UseHttpsRedirection();
```

Sau đó chạy lại và truy cập HTTP port 5000.

---

## ❌ Lỗi: "Cannot connect to SQL Server"

### Triệu chứng

Khi chạy `dotnet run`, thấy lỗi:

```
Microsoft.Data.SqlClient.SqlException: A network-related or instance-specific error...
```

### Giải pháp

#### Kiểm tra 1: SQL Server có đang chạy không?

**Nếu dùng Docker:**

```bash
# Kiểm tra container
docker ps | grep sqlserver

# Nếu không thấy, start container
docker start sqlserver

# Nếu chưa tạo container, tạo mới:
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest
```

**Nếu dùng SQL Server local (Windows):**

```powershell
# Kiểm tra service
Get-Service MSSQLSERVER

# Start service nếu stopped
Start-Service MSSQLSERVER
```

#### Kiểm tra 2: Connection String đúng chưa?

Mở `WebShop/appsettings.Development.json`:

**Nếu dùng Docker với password Admin@123456:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Nếu dùng SQL Server Windows Authentication:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiniMarketDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

**Nếu dùng SQL Server với username/password:**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiniMarketDB;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

#### Kiểm tra 3: Test connection trực tiếp

**Docker:**

```bash
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P "Admin@123456" \
   -Q "SELECT @@VERSION"
```

**Windows:**

```cmd
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

Nếu lỗi → SQL Server chưa chạy hoặc password sai.

---

## ❌ Lỗi: "A connection was successfully established... existing connection was forcibly closed"

### Giải pháp

Thêm `TrustServerCertificate=True` vào connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MiniMarketDB;Trusted_Connection=True;TrustServerCertificate=True;Encrypt=False"
  }
}
```

---

## ❌ Lỗi: Migration chưa được apply

### Triệu chứng

```
SqlException: Invalid object name 'Users'
```

### Giải pháp

```bash
# Chạy migrations thủ công
cd Persistence
dotnet ef database update --startup-project ../WebShop

# Nếu lỗi, xem danh sách migrations
dotnet ef migrations list --startup-project ../WebShop

# Nếu chưa có migrations, tạo mới
dotnet ef migrations add InitialCreate --startup-project ../WebShop
dotnet ef database update --startup-project ../WebShop
```

---

## ❌ Lỗi: Port already in use

### Triệu chứng

```
System.IO.IOException: Failed to bind to address http://127.0.0.1:5000
```

### Giải pháp

#### Option 1: Kill process đang dùng port

```bash
# macOS/Linux
lsof -ti:5000 | xargs kill -9
lsof -ti:5001 | xargs kill -9

# Windows
netstat -ano | findstr :5000
taskkill /PID <PID_NUMBER> /F
```

#### Option 2: Đổi port

```bash
dotnet run --urls "http://localhost:5500;https://localhost:5501"
```

Sau đó truy cập: http://localhost:5500

---

## ✅ QUICK FIX - Chạy nhanh nhất (Recommended)

Nếu gặp nhiều lỗi, làm theo thứ tự này:

### 1. Setup SQL Server qua Docker (Đơn giản nhất)

```bash
# Xóa container cũ nếu có
docker rm -f sqlserver

# Tạo container mới
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# Đợi 10 giây cho SQL Server khởi động
sleep 10

# Kiểm tra
docker logs sqlserver
```

### 2. Cập nhật Connection String

Mở `WebShop/appsettings.Development.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True;MultipleActiveResultSets=true;Encrypt=False"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
```

### 3. Clean và Setup lại

```bash
# Về root directory
cd /Users/dank/mini-market

# Clean (xóa migrations và database cũ)
cd Persistence
dotnet ef database drop --startup-project ../WebShop --force 2>/dev/null || true
rm -rf Migrations
cd ..

# Restore dependencies
dotnet restore

# Tạo migrations mới
cd Persistence
dotnet ef migrations add InitialCreate --startup-project ../WebShop

# Update database
dotnet ef database update --startup-project ../WebShop

cd ..
```

### 4. Build và Run

```bash
# Build
cd WebShop
dotnet build

# Run (chỉ dùng HTTP, không HTTPS để tránh lỗi SSL)
dotnet run --urls "http://localhost:5000"
```

### 5. Truy cập

Mở trình duyệt: **http://localhost:5000** (không dùng HTTPS, không có 's')

---

## 📋 Checklist Troubleshooting

Khi gặp lỗi, kiểm tra theo thứ tự:

- [ ] **SQL Server đang chạy?**

  ```bash
  docker ps | grep sqlserver  # Docker
  # hoặc
  Get-Service MSSQLSERVER     # Windows
  ```

- [ ] **Connection string đúng?**

  - Kiểm tra file `appsettings.Development.json`
  - Đảm bảo có `TrustServerCertificate=True`

- [ ] **Migrations đã được apply?**

  ```bash
  cd Persistence
  dotnet ef migrations list --startup-project ../WebShop
  ```

- [ ] **Port có bị chiếm không?**

  ```bash
  lsof -i :5000
  lsof -i :5001
  ```

- [ ] **.NET SDK version đúng?**

  ```bash
  dotnet --version
  # Cần >= 6.0
  ```

- [ ] **Build thành công?**
  ```bash
  cd WebShop
  dotnet build
  # Không có errors
  ```

---

## 🆘 Nếu vẫn không được

### Option 1: Sử dụng SQLite thay vì SQL Server

Thay đổi trong `Program.cs`:

```csharp
// Thay dòng này:
// builder.Services.AddDbContext<MiniMarketDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Bằng:
builder.Services.AddDbContext<MiniMarketDbContext>(options =>
    options.UseSqlite("Data Source=minimarket.db"));
```

Cài package SQLite:

```bash
cd Persistence
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

Sau đó clean và setup lại.

### Option 2: Gửi full error log

Chạy với verbose và lưu log:

```bash
cd WebShop
dotnet run --verbosity detailed > ~/Desktop/error.log 2>&1
```

Gửi file `error.log` để tôi xem chi tiết.

---

## 📞 Support Commands

### Xem tất cả processes của dotnet

```bash
ps aux | grep dotnet
```

### Kill tất cả dotnet processes

```bash
# macOS/Linux
pkill -9 dotnet

# Windows
taskkill /IM dotnet.exe /F
```

### Xem logs SQL Server (Docker)

```bash
docker logs sqlserver
docker logs --follow sqlserver  # Real-time
```

### Test connection tới SQL Server

```bash
# Docker
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P "Admin@123456" \
   -Q "SELECT DB_NAME()"
```

### Xem databases đã tạo

```bash
docker exec -it sqlserver /opt/mssql-tools/bin/sqlcmd \
   -S localhost -U sa -P "Admin@123456" \
   -Q "SELECT name FROM sys.databases"
```

---

## 🎯 Recommended Setup (Ít lỗi nhất)

```bash
# 1. SQL Server qua Docker
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# 2. Update connection string
cat > WebShop/appsettings.Development.json << 'EOF'
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True;Encrypt=False"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  }
}
EOF

# 3. Đợi SQL Server khởi động
sleep 15

# 4. Run setup
./setup.sh

# 5. Run app (chỉ HTTP)
cd WebShop
dotnet run --urls "http://localhost:5000"

# 6. Truy cập
# http://localhost:5000
```

Chúc may mắn! 🚀
