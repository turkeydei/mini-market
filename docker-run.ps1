$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}

Write-Host "=== MINI MARKET - DOCKER SETUP & RUN ===" -ForegroundColor Cyan
Write-Host ""

# Kiểm tra Docker
Write-Host "Bước 1: Kiểm tra Docker..." -ForegroundColor Yellow
try {
    $dockerVersion = docker --version
    Write-Host "✓ Docker đã được cài đặt: $dockerVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker chưa được cài đặt!" -ForegroundColor Red
    Write-Host "Vui lòng cài đặt Docker Desktop từ: https://www.docker.com/products/docker-desktop" -ForegroundColor Yellow
    exit 1
}

# Kiểm tra Docker Compose
Write-Host ""
Write-Host "Bước 2: Kiểm tra Docker Compose..." -ForegroundColor Yellow
try {
    $composeVersion = docker compose version
    Write-Host "✓ Docker Compose đã sẵn sàng: $composeVersion" -ForegroundColor Green
} catch {
    Write-Host "✗ Docker Compose không khả dụng!" -ForegroundColor Red
    exit 1
}

# Dừng và xóa containers cũ (nếu có)
Write-Host ""
Write-Host "Bước 3: Dọn dẹp containers cũ (nếu có)..." -ForegroundColor Yellow
docker compose down -v 2>$null
Write-Host "✓ Đã dọn dẹp" -ForegroundColor Green

# Build và chạy containers
Write-Host ""
Write-Host "Bước 4: Build và khởi động containers..." -ForegroundColor Yellow
Write-Host "Điều này có thể mất vài phút lần đầu tiên..." -ForegroundColor Gray

docker compose up --build -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "✗ Lỗi khi build/khởi động containers!" -ForegroundColor Red
    exit 1
}

Write-Host "✓ Containers đã được khởi động" -ForegroundColor Green

# Đợi SQL Server sẵn sàng
Write-Host ""
Write-Host "Bước 5: Đợi SQL Server sẵn sàng..." -ForegroundColor Yellow
$maxRetries = 30
$retryCount = 0
$sqlReady = $false

while ($retryCount -lt $maxRetries -and -not $sqlReady) {
    Start-Sleep -Seconds 2
    $retryCount++
    Write-Host "  Đang thử kết nối... ($retryCount/$maxRetries)" -ForegroundColor Gray
    
    $result = docker exec minimarket-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin@123456 -Q "SELECT 1" 2>&1
    if ($LASTEXITCODE -eq 0) {
        $sqlReady = $true
        Write-Host "✓ SQL Server đã sẵn sàng!" -ForegroundColor Green
    }
}

if (-not $sqlReady) {
    Write-Host "⚠ Cảnh báo: SQL Server có thể chưa sẵn sàng hoàn toàn" -ForegroundColor Yellow
}

# Chạy migrations
Write-Host ""
Write-Host "Bước 6: Chạy database migrations..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Kiểm tra logs
Write-Host ""
Write-Host "=== LOGS ===" -ForegroundColor Cyan
Write-Host "Để xem logs chi tiết: docker compose logs -f" -ForegroundColor Gray
Write-Host ""

# Hiển thị status
Write-Host "=== TRẠNG THÁI CONTAINERS ===" -ForegroundColor Cyan
docker compose ps

Write-Host ""
Write-Host "=== THÔNG TIN TRUY CẬP ===" -ForegroundColor Cyan
Write-Host "✓ SQL Server: localhost:1433" -ForegroundColor Green
Write-Host "✓ WebShop: http://localhost:5000" -ForegroundColor Green
Write-Host "✓ WebShop (HTTPS): https://localhost:5001" -ForegroundColor Green
Write-Host ""
Write-Host "=== CÁC LỆNH HỮU ÍCH ===" -ForegroundColor Cyan
Write-Host "  Xem logs:        docker compose logs -f" -ForegroundColor Gray
Write-Host "  Dừng containers: docker compose stop" -ForegroundColor Gray
Write-Host "  Xóa containers:  docker compose down" -ForegroundColor Gray
Write-Host "  Restart:         docker compose restart" -ForegroundColor Gray
Write-Host ""

