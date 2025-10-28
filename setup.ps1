# Script tự động setup và chạy Mini Market Project (Windows)
# Author: AI Assistant

$ErrorActionPreference = "Stop"

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MINI MARKET - SETUP & RUN SCRIPT" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if .NET SDK is installed
Write-Host "Bước 1: Kiểm tra .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK đã được cài đặt (Version: $dotnetVersion)" -ForegroundColor Green
} catch {
    Write-Host "✗ .NET SDK chưa được cài đặt!" -ForegroundColor Red
    Write-Host ""
    Write-Host "ℹ Vui lòng cài đặt .NET SDK từ: https://dotnet.microsoft.com/download" -ForegroundColor Blue
    exit 1
}

Write-Host ""
Write-Host "Bước 2: Kiểm tra EF Core Tools..." -ForegroundColor Yellow
try {
    $efVersion = dotnet ef --version
    Write-Host "✓ EF Core Tools đã được cài đặt" -ForegroundColor Green
} catch {
    Write-Host "⚠ EF Core Tools chưa được cài đặt. Đang cài đặt..." -ForegroundColor Yellow
    dotnet tool install --global dotnet-ef
    Write-Host "✓ Đã cài đặt EF Core Tools" -ForegroundColor Green
}

Write-Host ""
Write-Host "Bước 3: Restore dependencies..." -ForegroundColor Yellow
dotnet restore
Write-Host "✓ Dependencies đã được restore" -ForegroundColor Green

Write-Host ""
Write-Host "Bước 4: Xóa migrations cũ (nếu có)..." -ForegroundColor Yellow
Set-Location Persistence
if (Test-Path "Migrations") {
    Remove-Item -Recurse -Force Migrations
    Write-Host "✓ Đã xóa migrations cũ" -ForegroundColor Green
} else {
    Write-Host "ℹ Không có migrations cũ" -ForegroundColor Blue
}

Write-Host ""
Write-Host "Bước 5: Tạo migration mới..." -ForegroundColor Yellow
dotnet ef migrations add SimplifiedSchema --startup-project ..\WebShop
Write-Host "✓ Migration đã được tạo" -ForegroundColor Green

Write-Host ""
Write-Host "Bước 6: Kiểm tra SQL Server..." -ForegroundColor Yellow
Write-Host "⚠ Đảm bảo SQL Server đang chạy trên localhost" -ForegroundColor Yellow
Write-Host "ℹ Connection String: Server=localhost;Database=MiniMarketDB;Trusted_Connection=True" -ForegroundColor Blue

Write-Host ""
Write-Host "Nhấn Enter để tiếp tục update database (hoặc Ctrl+C để hủy)..." -ForegroundColor Cyan
$null = Read-Host

Write-Host ""
Write-Host "Bước 7: Update database..." -ForegroundColor Yellow
dotnet ef database update --startup-project ..\WebShop
Write-Host "✓ Database đã được cập nhật" -ForegroundColor Green

Set-Location ..

Write-Host ""
Write-Host "Bước 8: Build project..." -ForegroundColor Yellow
Set-Location WebShop
dotnet build
Write-Host "✓ Project đã được build thành công" -ForegroundColor Green

Set-Location ..

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  🎉 SETUP HOÀN TẤT!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "✓ Dữ liệu mẫu sẽ được tự động seed khi chạy ứng dụng" -ForegroundColor Green
Write-Host ""
Write-Host "ℹ Để chạy ứng dụng:" -ForegroundColor Blue
Write-Host "  cd WebShop" -ForegroundColor White
Write-Host "  dotnet run" -ForegroundColor White
Write-Host ""
Write-Host "ℹ Hoặc chạy trực tiếp:" -ForegroundColor Blue
Write-Host "  .\run.ps1" -ForegroundColor White
Write-Host ""
Write-Host "⚠ Database: MiniMarketDB" -ForegroundColor Yellow
Write-Host "⚠ Truy cập: https://localhost:5001 hoặc http://localhost:5000" -ForegroundColor Yellow
Write-Host ""

