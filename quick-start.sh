#!/bin/bash

# Script khởi động nhanh Mini Market với Docker
# Chạy script này sau khi đã cài Docker Desktop

set -e

echo "========================================="
echo "   🚀 MINI MARKET - QUICK START"
echo "========================================="
echo ""

# Colors
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

print_success() { echo -e "${GREEN}✓ $1${NC}"; }
print_error() { echo -e "${RED}✗ $1${NC}"; }
print_info() { echo -e "${BLUE}ℹ $1${NC}"; }
print_warning() { echo -e "${YELLOW}⚠ $1${NC}"; }

# Bước 1: Kiểm tra Docker
echo "Bước 1: Kiểm tra Docker..."
if ! command -v docker &> /dev/null; then
    print_error "Docker chưa được cài đặt hoặc chưa trong PATH!"
    print_info "Vui lòng khởi động Docker Desktop và chạy lại script này"
    exit 1
fi

if ! docker info > /dev/null 2>&1; then
    print_warning "Docker Desktop chưa khởi động!"
    print_info "Đang khởi động Docker Desktop..."
    open -a Docker
    echo "Đợi 30 giây cho Docker khởi động..."
    sleep 30
    
    # Kiểm tra lại
    if ! docker info > /dev/null 2>&1; then
        print_error "Docker vẫn chưa sẵn sàng. Vui lòng khởi động Docker Desktop thủ công và chạy lại."
        exit 1
    fi
fi

print_success "Docker đã sẵn sàng"

# Bước 2: Setup SQL Server
echo ""
echo "Bước 2: Setup SQL Server container..."

# Kiểm tra container đã tồn tại chưa
if docker ps -a --format '{{.Names}}' | grep -q '^sqlserver$'; then
    print_warning "Container 'sqlserver' đã tồn tại"
    
    # Kiểm tra container có đang chạy không
    if docker ps --format '{{.Names}}' | grep -q '^sqlserver$'; then
        print_success "SQL Server đang chạy"
    else
        print_info "Đang khởi động SQL Server container..."
        docker start sqlserver
        sleep 10
        print_success "SQL Server đã được khởi động"
    fi
else
    print_info "Đang tạo SQL Server container mới..."
    docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Admin@123456" \
       -p 1433:1433 --name sqlserver \
       -d mcr.microsoft.com/mssql/server:2022-latest
    
    print_info "Đợi SQL Server khởi động (20 giây)..."
    sleep 20
    print_success "SQL Server đã được tạo và khởi động"
fi

# Kiểm tra logs
echo ""
print_info "Kiểm tra SQL Server logs..."
docker logs sqlserver 2>&1 | tail -5

# Bước 3: Cập nhật Connection String
echo ""
echo "Bước 3: Cập nhật Connection String..."
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
print_success "Connection string đã được cập nhật"

# Bước 4: Restore dependencies
echo ""
echo "Bước 4: Restore dependencies..."
dotnet restore > /dev/null
print_success "Dependencies đã được restore"

# Bước 5: Setup Migrations
echo ""
echo "Bước 5: Setup Database Migrations..."

cd Persistence

# Xóa migrations cũ nếu có
if [ -d "Migrations" ]; then
    print_info "Xóa migrations cũ..."
    rm -rf Migrations
fi

# Tạo migrations mới
print_info "Tạo migrations mới..."
dotnet ef migrations add InitialCreate --startup-project ../WebShop > /dev/null

# Update database
print_info "Cập nhật database..."
dotnet ef database update --startup-project ../WebShop

print_success "Database đã được tạo và cập nhật"

cd ..

# Bước 6: Build project
echo ""
echo "Bước 6: Build project..."
cd WebShop
dotnet build > /dev/null
print_success "Project đã được build thành công"

# Hoàn tất
echo ""
echo "========================================="
echo "   ✅ SETUP HOÀN TẤT!"
echo "========================================="
echo ""
print_success "SQL Server đang chạy trên localhost:1433"
print_success "Database: MiniMarketDB"
print_success "Username: sa"
print_success "Password: Admin@123456"
echo ""
print_info "Để chạy ứng dụng:"
echo "  cd WebShop"
echo "  dotnet run --urls \"http://localhost:5000\""
echo ""
print_info "Hoặc chạy:"
echo "  ./run.sh"
echo ""
print_warning "Truy cập: http://localhost:5000"
echo ""
print_info "Dữ liệu mẫu sẽ tự động được seed khi chạy app lần đầu"
echo ""

