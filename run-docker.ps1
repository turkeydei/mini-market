# Script chạy ứng dụng Mini Market với Docker trên Windows
# Usage: .\run-docker.ps1

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  MINI MARKET - DOCKER RUN SCRIPT" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Bước 1: Kiểm tra Docker
Write-Host "Bước 1: Kiểm tra Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = & docker --version 2>&1
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Docker đã được cài đặt: $dockerVersion" -ForegroundColor Green
    } else {
        throw "Docker không khả dụng"
    }
} catch {
    Write-Host "✗ Docker chưa được cài đặt hoặc chưa chạy!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Vui lòng:" -ForegroundColor Yellow
    Write-Host "1. Cài đặt Docker Desktop từ: https://www.docker.com/products/docker-desktop/"
    Write-Host "2. Khởi động Docker Desktop"
    Write-Host "3. Chạy lại script này"
    exit 1
}

# Kiểm tra Docker Desktop đang chạy
Write-Host ""
Write-Host "Bước 2: Kiểm tra Docker Desktop đang chạy..." -ForegroundColor Yellow
try {
    & docker ps 2>&1 | Out-Null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Docker Desktop đang chạy" -ForegroundColor Green
    } else {
        throw "Docker không phản hồi"
    }
} catch {
    Write-Host "✗ Docker Desktop chưa khởi động!" -ForegroundColor Red
    Write-Host ""
    Write-Host "Vui lòng khởi động Docker Desktop và chạy lại script này." -ForegroundColor Yellow
    exit 1
}

# Bước 3: Dừng containers cũ (nếu có)
Write-Host ""
Write-Host "Bước 3: Dọn dẹp containers cũ (nếu có)..." -ForegroundColor Yellow
& docker-compose down 2>&1 | Out-Null
Write-Host "✓ Đã dọn dẹp" -ForegroundColor Green

# Bước 4: Build và chạy containers
Write-Host ""
Write-Host "Bước 4: Build và chạy containers..." -ForegroundColor Yellow
Write-Host "⚠️  Lần đầu chạy sẽ mất 5-10 phút để tải images và build" -ForegroundColor Yellow
Write-Host ""

& docker-compose up -d --build

if ($LASTEXITCODE -eq 0) {
    Write-Host ""
    Write-Host "✓ Containers đã được khởi động!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "✗ Có lỗi khi khởi động containers!" -ForegroundColor Red
    Write-Host "Xem logs để biết thêm chi tiết: docker-compose logs" -ForegroundColor Yellow
    exit 1
}

# Bước 5: Chờ containers khởi động
Write-Host ""
Write-Host "Bước 5: Đợi containers khởi động hoàn toàn..." -ForegroundColor Yellow
Write-Host "Đang chờ 15 giây..." -ForegroundColor Gray
Start-Sleep -Seconds 15

# Bước 6: Kiểm tra trạng thái
Write-Host ""
Write-Host "Bước 6: Kiểm tra trạng thái containers..." -ForegroundColor Yellow
& docker ps --filter "name=minimarket" --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"

# Bước 7: Kiểm tra logs
Write-Host ""
Write-Host "Bước 7: Kiểm tra logs ứng dụng..." -ForegroundColor Yellow
Write-Host "Đang tải logs (10 dòng cuối)..." -ForegroundColor Gray
Write-Host ""
$logs = & docker logs minimarket-webshop --tail 10 2>&1
Write-Host $logs

# Kiểm tra xem ứng dụng đã sẵn sàng chưa
if ($logs -match "Now listening on" -or $logs -match "Application started") {
    Write-Host ""
    Write-Host "✓ Ứng dụng đã sẵn sàng!" -ForegroundColor Green
} else {
    Write-Host ""
    Write-Host "⚠️  Ứng dụng đang khởi động, vui lòng đợi thêm..." -ForegroundColor Yellow
    Write-Host "Xem logs real-time: docker logs minimarket-webshop -f" -ForegroundColor Gray
}

# Thông tin truy cập
Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  🎉 HOÀN TẤT!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "🌐 Truy cập ứng dụng:" -ForegroundColor Green
Write-Host "   - http://localhost:5000" -ForegroundColor Yellow
Write-Host "   - https://localhost:5001" -ForegroundColor Yellow
Write-Host ""
Write-Host "🔑 Tài khoản demo:" -ForegroundColor Green
Write-Host "   - Admin: admin / Admin@123" -ForegroundColor Yellow
Write-Host "   - User:  user1 / User@123" -ForegroundColor Yellow
Write-Host ""
Write-Host "📋 Các lệnh hữu ích:" -ForegroundColor Green
Write-Host "   - Xem logs:        docker logs minimarket-webshop -f" -ForegroundColor Gray
Write-Host "   - Dừng containers: docker-compose down" -ForegroundColor Gray
Write-Host "   - Khởi động lại:   docker-compose up -d" -ForegroundColor Gray
Write-Host "   - Xem trạng thái:  docker ps" -ForegroundColor Gray
Write-Host ""

