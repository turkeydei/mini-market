# Script cập nhật migrations từ đồng nghiệp
# Usage: .\update-migrations.ps1

$ErrorActionPreference = "Stop"
try { [Console]::OutputEncoding = [System.Text.Encoding]::UTF8 } catch {}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CẬP NHẬT MIGRATIONS TỪ ĐỒNG NGHIỆP" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Kiểm tra thư mục migrations
$migrationsPath = "Persistence\Migrations"
if (-not (Test-Path $migrationsPath)) {
    Write-Host "✗ Không tìm thấy thư mục Migrations!" -ForegroundColor Red
    exit 1
}

Write-Host "Bước 1: Kiểm tra migrations hiện tại..." -ForegroundColor Yellow
$currentMigrations = Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File | 
    Where-Object { $_.Name -match '^\d{14}_' } | 
    Sort-Object Name

Write-Host "✓ Tìm thấy $($currentMigrations.Count) migration(s) hiện tại:" -ForegroundColor Green
foreach ($migration in $currentMigrations) {
    Write-Host "   - $($migration.Name)" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Bước 2: Hướng dẫn nhận file từ đồng nghiệp..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Đồng nghiệp cần gửi bạn các file sau:" -ForegroundColor Cyan
Write-Host "  1. File migration mới (.cs và .Designer.cs)" -ForegroundColor White
Write-Host "  2. File MiniMarketDbContextModelSnapshot.cs (đã cập nhật)" -ForegroundColor White
Write-Host ""
Write-Host "Ví dụ:" -ForegroundColor Cyan
Write-Host "  - 20251125120000_AddNewColumn.cs" -ForegroundColor Gray
Write-Host "  - 20251125120000_AddNewColumn.Designer.cs" -ForegroundColor Gray
Write-Host "  - MiniMarketDbContextModelSnapshot.cs" -ForegroundColor Gray
Write-Host ""

# Kiểm tra xem có file migration mới không
Write-Host "Bước 3: Kiểm tra file migration mới..." -ForegroundColor Yellow
Write-Host ""
Write-Host "⚠️  Vui lòng đặt các file migration mới vào thư mục:" -ForegroundColor Yellow
Write-Host "   $((Get-Location).Path)\$migrationsPath" -ForegroundColor Cyan
Write-Host ""
$continue = Read-Host "Đã copy file xong chưa? (Y/N)"

if ($continue -ne "Y" -and $continue -ne "y") {
    Write-Host "Hủy bỏ. Vui lòng copy file và chạy lại script." -ForegroundColor Yellow
    exit 0
}

# Kiểm tra file migration mới
Write-Host ""
Write-Host "Bước 4: Kiểm tra file migration mới..." -ForegroundColor Yellow
$allMigrations = Get-ChildItem -Path $migrationsPath -Filter "*.cs" -File | 
    Where-Object { $_.Name -match '^\d{14}_' } | 
    Sort-Object Name

$newMigrations = $allMigrations | Where-Object { 
    $migrationName = $_.BaseName
    $designerExists = Test-Path "$migrationsPath\$migrationName.Designer.cs"
    return $designerExists
}

if ($newMigrations.Count -eq 0) {
    Write-Host "⚠️  Không tìm thấy migration mới hoặc thiếu file .Designer.cs" -ForegroundColor Yellow
    Write-Host "Vui lòng kiểm tra lại:" -ForegroundColor Yellow
    Write-Host "  - File migration phải có cả .cs và .Designer.cs" -ForegroundColor Gray
    Write-Host "  - Tên file phải đúng format: YYYYMMDDHHMMSS_TenMigration" -ForegroundColor Gray
} else {
    Write-Host "✓ Tìm thấy $($newMigrations.Count) migration(s) mới:" -ForegroundColor Green
    foreach ($migration in $newMigrations) {
        Write-Host "   - $($migration.Name)" -ForegroundColor Gray
        Write-Host "   - $($migration.BaseName).Designer.cs" -ForegroundColor Gray
    }
}

# Kiểm tra ModelSnapshot
Write-Host ""
Write-Host "Bước 5: Kiểm tra ModelSnapshot..." -ForegroundColor Yellow
$snapshotPath = "$migrationsPath\MiniMarketDbContextModelSnapshot.cs"
if (Test-Path $snapshotPath) {
    Write-Host "✓ Tìm thấy ModelSnapshot" -ForegroundColor Green
    $snapshotInfo = Get-Item $snapshotPath
    Write-Host "   Cập nhật lần cuối: $($snapshotInfo.LastWriteTime)" -ForegroundColor Gray
} else {
    Write-Host "⚠️  Không tìm thấy ModelSnapshot!" -ForegroundColor Yellow
    Write-Host "   Vui lòng đảm bảo có file MiniMarketDbContextModelSnapshot.cs" -ForegroundColor Yellow
}

# Xác nhận cập nhật database
Write-Host ""
Write-Host "Bước 6: Cập nhật database..." -ForegroundColor Yellow
Write-Host ""
Write-Host "Bạn muốn:" -ForegroundColor Cyan
Write-Host "  1. Reset database và tạo lại (xóa dữ liệu cũ)" -ForegroundColor White
Write-Host "  2. Chỉ apply migrations mới (giữ dữ liệu cũ)" -ForegroundColor White
Write-Host "  3. Chỉ kiểm tra, không cập nhật" -ForegroundColor White
Write-Host ""
$choice = Read-Host "Chọn (1/2/3)"

switch ($choice) {
    "1" {
        Write-Host ""
        Write-Host "⚠️  CẢNH BÁO: Sẽ xóa toàn bộ dữ liệu!" -ForegroundColor Red
        $confirm = Read-Host "Bạn có chắc chắn? (yes/no)"
        if ($confirm -eq "yes") {
            Write-Host ""
            Write-Host "Đang dừng containers và xóa volumes..." -ForegroundColor Yellow
            docker compose down -v
            Write-Host "✓ Đã xóa volumes" -ForegroundColor Green
            
            Write-Host ""
            Write-Host "Đang khởi động lại containers..." -ForegroundColor Yellow
            docker compose up -d
            Write-Host "✓ Đã khởi động containers" -ForegroundColor Green
            
            Write-Host ""
            Write-Host "Đợi 20 giây để SQL Server sẵn sàng..." -ForegroundColor Yellow
            Start-Sleep -Seconds 20
            
            Write-Host ""
            Write-Host "Kiểm tra logs..." -ForegroundColor Yellow
            docker logs minimarket-webshop --tail 30
        } else {
            Write-Host "Đã hủy." -ForegroundColor Yellow
        }
    }
    "2" {
        Write-Host ""
        Write-Host "Đang apply migrations mới..." -ForegroundColor Yellow
        Write-Host "⚠️  Nếu đang chạy Docker, migrations sẽ tự động apply khi container khởi động" -ForegroundColor Yellow
        Write-Host ""
        Write-Host "Để apply migrations thủ công:" -ForegroundColor Cyan
        Write-Host "  docker exec -it minimarket-webshop dotnet ef database update --project Persistence" -ForegroundColor Gray
        Write-Host ""
        Write-Host "Hoặc restart container:" -ForegroundColor Cyan
        Write-Host "  docker compose restart webshop" -ForegroundColor Gray
    }
    "3" {
        Write-Host ""
        Write-Host "✓ Chỉ kiểm tra, không cập nhật database" -ForegroundColor Green
        Write-Host ""
        Write-Host "Để cập nhật sau, chạy:" -ForegroundColor Cyan
        Write-Host "  docker compose down -v" -ForegroundColor Gray
        Write-Host "  docker compose up -d" -ForegroundColor Gray
    }
    default {
        Write-Host "Lựa chọn không hợp lệ!" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  HOÀN TẤT!" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "📋 Tóm tắt:" -ForegroundColor Green
Write-Host "  - Migrations hiện tại: $($currentMigrations.Count)" -ForegroundColor Gray
Write-Host "  - Migrations mới: $($newMigrations.Count)" -ForegroundColor Gray
Write-Host ""
Write-Host "📖 Xem hướng dẫn chi tiết: MIGRATION_GUIDE.md" -ForegroundColor Cyan
Write-Host ""

