#!/bin/bash

# Script để clean database và migrations

echo "🧹 Đang dọn dẹp project..."
echo ""

# Xóa database
echo "Xóa database..."
cd Persistence
dotnet ef database drop --startup-project ../WebShop --force
echo "✓ Đã xóa database"

# Xóa migrations
echo "Xóa migrations..."
rm -rf Migrations
echo "✓ Đã xóa migrations"

cd ..

echo ""
echo "✅ Dọn dẹp hoàn tất!"
echo "Chạy ./setup.sh để setup lại từ đầu"

