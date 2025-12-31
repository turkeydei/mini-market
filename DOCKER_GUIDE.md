# 🐳 Hướng Dẫn Chạy Đồ Án Với Docker

## 📋 Yêu Cầu

1. **Docker Desktop** đã được cài đặt và đang chạy
   - Download: https://www.docker.com/products/docker-desktop
   - Đảm bảo Docker Desktop đang chạy trước khi chạy lệnh

2. **Docker Compose** (thường đi kèm với Docker Desktop)

## 🚀 Cách Chạy Nhanh

### macOS/Linux:
```bash
# Cấp quyền thực thi (chỉ cần làm 1 lần)
chmod +x docker-start.sh

# Chạy script tự động
./docker-start.sh
```

### Windows (PowerShell):
```powershell
# Chạy script PowerShell
.\run-docker.ps1
```

### Hoặc chạy thủ công:
```bash
# Build và khởi động
docker-compose up -d --build

# Hoặc nếu dùng Docker Compose V2
docker compose up -d --build
```

## 📦 Các Services

### 1. SQL Server
- **Container**: `minimarket-sqlserver`
- **Port**: `1433`
- **Database**: `MiniMarketDB`
- **Username**: `sa`
- **Password**: `Admin@123456`

### 2. WebShop Application
- **Container**: `minimarket-webshop`
- **Port**: `5050` (HTTP)
- **URL**: http://localhost:5050
- **Admin Panel**: http://localhost:5050/Admin

## 🔧 Các Lệnh Docker Hữu Ích

### Xem trạng thái containers:
```bash
docker-compose ps
# hoặc
docker ps
```

### Xem logs:
```bash
# Logs của WebShop
docker logs minimarket-webshop -f

# Logs của SQL Server
docker logs minimarket-sqlserver -f

# Logs của tất cả services
docker-compose logs -f
```

### Dừng containers:
```bash
docker-compose stop
```

### Khởi động lại containers:
```bash
docker-compose restart
```

### Dừng và xóa containers (giữ volumes):
```bash
docker-compose down
```

### Dừng và xóa tất cả (bao gồm volumes):
```bash
docker-compose down -v
```

### Rebuild và khởi động lại:
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

## 🐛 Troubleshooting

### 1. Lỗi "Cannot connect to Docker daemon"
**Nguyên nhân**: Docker Desktop chưa khởi động
**Giải pháp**: 
- Mở Docker Desktop
- Đợi Docker khởi động hoàn toàn
- Chạy lại lệnh

### 2. Lỗi "Port already in use"
**Nguyên nhân**: Port 5050 hoặc 1433 đã được sử dụng
**Giải pháp**:
```bash
# Kiểm tra port đang được sử dụng
lsof -i :5050
lsof -i :1433

# Hoặc thay đổi port trong docker-compose.yml
```

### 3. Lỗi "SQL Server connection failed"
**Nguyên nhân**: SQL Server chưa sẵn sàng
**Giải pháp**:
```bash
# Kiểm tra SQL Server logs
docker logs minimarket-sqlserver

# Đợi thêm thời gian (SQL Server cần 30-60 giây để khởi động)
# Kiểm tra lại sau 1 phút
```

### 4. Lỗi "Build failed"
**Nguyên nhân**: Có thể do cache hoặc network
**Giải pháp**:
```bash
# Xóa cache và build lại
docker-compose down
docker system prune -f
docker-compose build --no-cache
docker-compose up -d
```

### 5. Ứng dụng không truy cập được
**Kiểm tra**:
```bash
# Xem logs của WebShop
docker logs minimarket-webshop

# Kiểm tra container có đang chạy không
docker ps | grep minimarket-webshop

# Kiểm tra port mapping
docker port minimarket-webshop
```

## 📝 Cấu Hình

### Connection String
Connection string được cấu hình trong `docker-compose.yml`:
```
Server=sqlserver,1433;Database=MiniMarketDB;User Id=sa;Password=Admin@123456;TrustServerCertificate=True;Encrypt=False
```

### Environment Variables
- `ASPNETCORE_ENVIRONMENT=Development`
- `ASPNETCORE_URLS=http://+:8080`

### Ports
- **5050**: WebShop HTTP (mapped từ 8080 trong container)
- **5051**: WebShop HTTPS (mapped từ 8081 trong container)
- **1433**: SQL Server

## 🔐 Tài Khoản Test

### Admin:
- **Email**: `admin@minimarket.com`
- **Password**: `Admin@123`

### Customer:
- **Email**: `user1@minimarket.com`
- **Password**: `User@123`

## 📊 Database

Database sẽ tự động được tạo và migrate khi container WebShop khởi động lần đầu. Seed data cũng sẽ tự động được thêm vào.

## 🎯 Truy Cập Ứng Dụng

1. **Trang chủ**: http://localhost:5050
2. **Admin Panel**: http://localhost:5050/Admin
3. **Login**: http://localhost:5050/Auth/Login

## 📚 Tài Liệu Thêm

- Xem `DOCKER_STATUS.md` để biết trạng thái hiện tại
- Xem `README.md` để biết thêm về dự án

