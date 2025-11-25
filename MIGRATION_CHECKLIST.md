# Checklist: Gửi Migration cho Đồng Nghiệp

## 📋 Checklist cho đồng nghiệp (người tạo migration)

Khi bạn đã tạo migration mới và muốn gửi cho đồng nghiệp, hãy đảm bảo gửi đầy đủ các file sau:

### ✅ Bắt buộc phải gửi:

- [ ] **File Migration chính** 
  - `Persistence/Migrations/YYYYMMDDHHMMSS_TenMigration.cs`
  - Ví dụ: `20251125120000_AddEmailToUser.cs`

- [ ] **File Migration Designer**
  - `Persistence/Migrations/YYYYMMDDHHMMSS_TenMigration.Designer.cs`
  - Ví dụ: `20251125120000_AddEmailToUser.Designer.cs`
  - ⚠️ **QUAN TRỌNG**: Phải có cả 2 file (.cs và .Designer.cs)

- [ ] **File ModelSnapshot (đã cập nhật)**
  - `Persistence/Migrations/MiniMarketDbContextModelSnapshot.cs`
  - ⚠️ **QUAN TRỌNG**: Phải là file mới nhất sau khi tạo migration

### 📝 Tùy chọn (nếu có thay đổi):

- [ ] **File Entity đã thay đổi**
  - Các file trong `Domain/Entities/` nếu có thay đổi model
  - Ví dụ: `Domain/Entities/User.cs` (nếu thêm cột mới)

### 🚫 KHÔNG cần gửi:

- ❌ File migration cũ (đã có sẵn)
- ❌ File bin/obj (sẽ tự build)
- ❌ File .csproj (trừ khi thêm package mới)

---

## 📦 Cách đóng gói để gửi

### Cách 1: Zip thư mục Migrations
```powershell
# Tạo file zip chứa migrations mới
Compress-Archive -Path "Persistence\Migrations\20251125*" -DestinationPath "migrations-new.zip"
Compress-Archive -Path "Persistence\Migrations\MiniMarketDbContextModelSnapshot.cs" -DestinationPath "migrations-new.zip" -Update
```

### Cách 2: Copy file thủ công
Copy các file sau vào một thư mục và nén lại:
- Tất cả file migration mới (có timestamp mới nhất)
- File ModelSnapshot.cs

### Cách 3: Dùng Git (khuyến nghị)
```bash
# Commit và push migrations
git add Persistence/Migrations/
git commit -m "Add migration: TenMigrationMoi"
git push
```

---

## 📧 Mẫu tin nhắn gửi đồng nghiệp

```
Hi [Tên đồng nghiệp],

Mình vừa tạo migration mới: [Tên migration]
- Thay đổi: [Mô tả ngắn gọn]
- Files đính kèm:
  + 20251125120000_TenMigration.cs
  + 20251125120000_TenMigration.Designer.cs
  + MiniMarketDbContextModelSnapshot.cs

Bạn copy các file này vào thư mục Persistence/Migrations/ 
và chạy: docker compose down -v && docker compose up -d

Cảm ơn!
```

---

## ✅ Checklist cho người nhận (bạn)

Khi nhận migration từ đồng nghiệp:

- [ ] Đã nhận đủ 3 file (hoặc nhiều hơn nếu có nhiều migration)
- [ ] Đã copy vào đúng thư mục `Persistence/Migrations/`
- [ ] Đã backup database (nếu có dữ liệu quan trọng)
- [ ] Đã chạy script `update-migrations.ps1` hoặc `docker compose down -v`
- [ ] Đã kiểm tra logs để đảm bảo migrations apply thành công
- [ ] Đã test ứng dụng hoạt động bình thường

---

## 🔍 Cách kiểm tra migration đã được apply

```powershell
# Xem danh sách migrations
docker exec minimarket-webshop dotnet ef migrations list --project Persistence

# Hoặc kiểm tra trong database
docker exec minimarket-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Admin@123456 -Q "SELECT * FROM [MiniMarketDB].[dbo].[__EFMigrationsHistory]"
```

---

## ⚠️ Lưu ý quan trọng

1. **KHÔNG BAO GIỜ** sửa file migration đã được apply vào database
2. **LUÔN** gửi cả file .cs và .Designer.cs
3. **LUÔN** cập nhật ModelSnapshot
4. **KIỂM TRA** tên file migration không trùng với migration cũ
5. **BACKUP** database trước khi apply nếu có dữ liệu quan trọng

---

## 🆘 Xử lý lỗi thường gặp

### Lỗi: "Migration already applied"
- **Nguyên nhân**: Migration đã được apply trước đó
- **Giải pháp**: Bỏ qua migration đó hoặc reset database

### Lỗi: "Missing Designer file"
- **Nguyên nhân**: Thiếu file .Designer.cs
- **Giải pháp**: Yêu cầu đồng nghiệp gửi lại đầy đủ

### Lỗi: "ModelSnapshot out of date"
- **Nguyên nhân**: ModelSnapshot không khớp với migrations
- **Giải pháp**: Yêu cầu đồng nghiệp gửi lại ModelSnapshot mới nhất

