# Hướng dẫn cập nhật Database Migration

## Khi đồng nghiệp sửa database

### 1. Đồng nghiệp cần gửi bạn các file sau:

**Bắt buộc:**
- Tất cả các file migration mới trong thư mục `Persistence/Migrations/`
  - `YYYYMMDDHHMMSS_TenMigration.cs` (file migration chính)
  - `YYYYMMDDHHMMSS_TenMigration.Designer.cs` (file metadata)
  
**Quan trọng:**
- `Persistence/Migrations/MiniMarketDbContextModelSnapshot.cs` (file snapshot - cập nhật mới nhất)

**Có thể cần:**
- Các file Entity đã thay đổi trong `Domain/Entities/` (nếu có thay đổi model)

### 2. Cách cập nhật migration từ đồng nghiệp

#### Bước 1: Nhận file từ đồng nghiệp
Đồng nghiệp sẽ gửi bạn:
- File migration mới (ví dụ: `20251125120000_AddNewColumn.cs` và `.Designer.cs`)
- File `MiniMarketDbContextModelSnapshot.cs` đã cập nhật

#### Bước 2: Copy file vào project
```powershell
# Copy file migration mới vào thư mục Migrations
# Ví dụ: Copy 20251125120000_AddNewColumn.cs và .Designer.cs vào Persistence/Migrations/
```

#### Bước 3: Cập nhật database
```powershell
# Nếu đang chạy với Docker
docker compose down
docker compose up -d

# Hoặc nếu chạy local
dotnet ef database update --project Persistence --startup-project WebShop
```

### 3. Quy trình đầy đủ (cho đồng nghiệp)

Khi đồng nghiệp sửa database, họ cần làm:

```powershell
# 1. Tạo migration mới
dotnet ef migrations add TenMigrationMoi --project Persistence --startup-project WebShop

# 2. Gửi các file sau cho bạn:
# - Persistence/Migrations/YYYYMMDDHHMMSS_TenMigrationMoi.cs
# - Persistence/Migrations/YYYYMMDDHHMMSS_TenMigrationMoi.Designer.cs
# - Persistence/Migrations/MiniMarketDbContextModelSnapshot.cs
```

### 4. Cách kiểm tra migration mới

Sau khi copy file, kiểm tra:
```powershell
# Xem danh sách migrations
dotnet ef migrations list --project Persistence --startup-project WebShop
```

### 5. Lưu ý quan trọng

⚠️ **KHÔNG BAO GIỜ:**
- Xóa file migration cũ
- Sửa file migration đã được apply vào database
- Chỉ copy một phần file migration

✅ **NÊN:**
- Copy đầy đủ cả 2 file (.cs và .Designer.cs)
- Luôn cập nhật ModelSnapshot
- Backup database trước khi update (nếu có dữ liệu quan trọng)

### 6. Ví dụ cụ thể

**Tình huống:** Đồng nghiệp thêm cột `Email` vào bảng `User`

**Đồng nghiệp sẽ gửi:**
```
Persistence/Migrations/
  ├── 20251125120000_AddEmailToUser.cs
  ├── 20251125120000_AddEmailToUser.Designer.cs
  └── MiniMarketDbContextModelSnapshot.cs (đã cập nhật)
```

**Bạn sẽ:**
1. Copy 3 file trên vào project
2. Chạy `docker compose down -v` (nếu muốn reset database)
3. Chạy `docker compose up -d` (tự động apply migrations)

### 7. Script tự động (tùy chọn)

Nếu muốn tự động hóa, có thể tạo script để:
- Kiểm tra migration mới
- Backup database
- Apply migrations
- Verify kết quả

