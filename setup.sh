#!/bin/bash

# Script tự động setup và chạy Mini Market Project
# Author: AI Assistant
# Date: $(date +%Y-%m-%d)

set -e  # Exit on error

echo "========================================"
echo "  MINI MARKET - SETUP & RUN SCRIPT"
echo "========================================"
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Function to print colored output
print_success() {
    echo -e "${GREEN}✓ $1${NC}"
}

print_error() {
    echo -e "${RED}✗ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠ $1${NC}"
}

# Check if .NET SDK is installed
echo "Bước 1: Kiểm tra .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK chưa được cài đặt!"
    echo ""
    print_info "Vui lòng cài đặt .NET SDK từ: https://dotnet.microsoft.com/download"
    print_info "Hoặc sử dụng Homebrew: brew install --cask dotnet-sdk"
    exit 1
else
    DOTNET_VERSION=$(dotnet --version)
    print_success ".NET SDK đã được cài đặt (Version: $DOTNET_VERSION)"
fi

echo ""
echo "Bước 2: Kiểm tra EF Core Tools..."
if ! dotnet ef --version &> /dev/null; then
    print_warning "EF Core Tools chưa được cài đặt. Đang cài đặt..."
    dotnet tool install --global dotnet-ef
    print_success "Đã cài đặt EF Core Tools"
else
    EF_VERSION=$(dotnet ef --version | head -n 1)
    print_success "EF Core Tools đã được cài đặt ($EF_VERSION)"
fi

echo ""
echo "Bước 3: Restore dependencies..."
dotnet restore
print_success "Dependencies đã được restore"

echo ""
echo "Bước 4: Xóa migrations cũ (nếu có)..."
cd Persistence
if [ -d "Migrations" ]; then
    rm -rf Migrations
    print_success "Đã xóa migrations cũ"
else
    print_info "Không có migrations cũ"
fi

echo ""
echo "Bước 5: Tạo migration mới..."
dotnet ef migrations add SimplifiedSchema --startup-project ../WebShop
print_success "Migration đã được tạo"

echo ""
echo "Bước 6: Kiểm tra SQL Server..."
print_warning "Đảm bảo SQL Server đang chạy trên localhost"
print_info "Connection String: Server=localhost;Database=MiniMarketDB;Trusted_Connection=True"

echo ""
read -p "Nhấn Enter để tiếp tục update database (hoặc Ctrl+C để hủy)..."

echo ""
echo "Bước 7: Update database..."
dotnet ef database update --startup-project ../WebShop
print_success "Database đã được cập nhật"

cd ..

echo ""
echo "Bước 8: Build project..."
cd WebShop
dotnet build
print_success "Project đã được build thành công"

echo ""
echo "========================================"
echo "  🎉 SETUP HOÀN TẤT!"
echo "========================================"
echo ""
print_success "Dữ liệu mẫu sẽ được tự động seed khi chạy ứng dụng"
echo ""
print_info "Để chạy ứng dụng:"
echo "  cd WebShop"
echo "  dotnet run"
echo ""
print_info "Hoặc chạy trực tiếp:"
echo "  ./run.sh"
echo ""
print_warning "Database: MiniMarketDB"
print_warning "Truy cập: https://localhost:5001 hoặc http://localhost:5000"
echo ""

