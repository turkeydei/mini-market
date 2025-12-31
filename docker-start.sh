#!/bin/bash

# Script khởi động Mini Market với Docker Compose
# Sử dụng script này để chạy toàn bộ ứng dụng trong Docker

set -e

echo "========================================="
echo "   🐳 MINI MARKET - DOCKER START"
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

# Tìm Docker ở các vị trí phổ biến trên macOS
DOCKER_CMD=""
if command -v docker &> /dev/null; then
    DOCKER_CMD="docker"
elif [ -f "/usr/local/bin/docker" ]; then
    DOCKER_CMD="/usr/local/bin/docker"
    export PATH="/usr/local/bin:$PATH"
elif [ -f "/Applications/Docker.app/Contents/Resources/bin/docker" ]; then
    DOCKER_CMD="/Applications/Docker.app/Contents/Resources/bin/docker"
    export PATH="/Applications/Docker.app/Contents/Resources/bin:$PATH"
fi

if [ -z "$DOCKER_CMD" ]; then
    print_error "Docker chưa được cài đặt hoặc không tìm thấy!"
    print_info "Vui lòng:"
    print_info "1. Đảm bảo Docker Desktop đã được cài đặt"
    print_info "2. Khởi động lại terminal hoặc chạy: export PATH=\"/usr/local/bin:\$PATH\""
    print_info "3. Hoặc thêm Docker vào PATH trong ~/.zshrc"
    exit 1
fi

print_success "Đã tìm thấy Docker: $DOCKER_CMD"

# Kiểm tra Docker Compose
DOCKER_COMPOSE_CMD=""
if command -v docker-compose &> /dev/null; then
    DOCKER_COMPOSE_CMD="docker-compose"
elif $DOCKER_CMD compose version &> /dev/null; then
    DOCKER_COMPOSE_CMD="$DOCKER_CMD compose"
else
    print_warning "Docker Compose không tìm thấy, sẽ thử dùng 'docker compose'"
    DOCKER_COMPOSE_CMD="$DOCKER_CMD compose"
fi

if ! $DOCKER_CMD info > /dev/null 2>&1; then
    print_warning "Docker Desktop chưa khởi động!"
    print_info "Đang khởi động Docker Desktop..."
    open -a Docker 2>/dev/null || true
    echo "Đợi 30 giây cho Docker khởi động..."
    sleep 30
    
    if ! $DOCKER_CMD info > /dev/null 2>&1; then
        print_error "Docker vẫn chưa sẵn sàng. Vui lòng khởi động Docker Desktop thủ công."
        exit 1
    fi
fi

print_success "Docker đã sẵn sàng"

# Bước 2: Dừng containers cũ nếu có
echo ""
echo "Bước 2: Dọn dẹp containers cũ..."
if $DOCKER_CMD ps -a | grep -q "minimarket"; then
    print_info "Đang dừng containers cũ..."
    $DOCKER_COMPOSE_CMD down 2>/dev/null || true
    print_success "Đã dọn dẹp containers cũ"
fi

# Bước 3: Build và khởi động
echo ""
echo "Bước 3: Build và khởi động containers..."
print_info "Đang build Docker images (có thể mất vài phút lần đầu)..."
$DOCKER_COMPOSE_CMD build --no-cache

echo ""
print_info "Đang khởi động containers..."
$DOCKER_COMPOSE_CMD up -d

print_success "Containers đã được khởi động"

# Bước 4: Đợi SQL Server sẵn sàng
echo ""
echo "Bước 4: Đợi SQL Server khởi động..."
print_info "Đang đợi SQL Server sẵn sàng (có thể mất 30-60 giây)..."
sleep 10

for i in {1..30}; do
    if $DOCKER_CMD exec minimarket-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "Admin@123456" -Q "SELECT 1" -C &>/dev/null; then
        print_success "SQL Server đã sẵn sàng"
        break
    fi
    if [ $i -eq 30 ]; then
        print_warning "SQL Server chưa sẵn sàng sau 60 giây, nhưng sẽ tiếp tục..."
    else
        echo -n "."
        sleep 2
    fi
done

# Bước 5: Kiểm tra logs
echo ""
echo "Bước 5: Kiểm tra trạng thái..."
echo ""
print_info "Containers đang chạy:"
$DOCKER_COMPOSE_CMD ps 2>/dev/null || $DOCKER_CMD ps --filter "name=minimarket"

echo ""
print_info "Logs của WebShop (10 dòng cuối):"
$DOCKER_CMD logs minimarket-webshop --tail 10 2>/dev/null || print_warning "Chưa có logs"

# Hoàn tất
echo ""
echo "========================================="
echo "   ✅ DOCKER SETUP HOÀN TẤT!"
echo "========================================="
echo ""
print_success "SQL Server: localhost:1433"
print_success "WebShop: http://localhost:5050"
print_success "Database: MiniMarketDB"
print_success "Username: sa"
print_success "Password: Admin@123456"
echo ""
print_info "Để xem logs:"
echo "  $DOCKER_CMD logs minimarket-webshop -f"
echo "  $DOCKER_CMD logs minimarket-sqlserver -f"
echo ""
print_info "Để dừng containers:"
echo "  $DOCKER_COMPOSE_CMD down"
echo ""
print_info "Để khởi động lại:"
echo "  $DOCKER_COMPOSE_CMD up -d"
echo ""
print_warning "Truy cập ứng dụng: http://localhost:5050"
print_warning "Admin Panel: http://localhost:5050/Admin"
echo ""
print_info "Tài khoản test:"
echo "  Admin: admin@minimarket.com / Admin@123"
echo "  User: user1@minimarket.com / User@123"
echo ""

