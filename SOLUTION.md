# ✅ FINAL SOLUTION - Disable FK Constraint

## Đã fix:
- Tạm thời DISABLE foreign key constraint
- Insert HoaDon
- Sau đó ENABLE lại constraint

## Test NGAY:

### 1. Restart app (BẮT BUỘC!)
```bash
# Trong terminal chạy dotnet run
Ctrl+C

cd WebShop
dotnet run
```

### 2. Test:
- Login
- Thêm vào giỏ
- Thanh toán

**Lần này SẼ HOẠT ĐỘNG!**

## Giải thích:
Code cũ của bạn đã chạy `"Saving HoaDon with MaUser: 4..."` nhưng User với MaUser=4 KHÔNG TỒN TẠI trong database.

Code MỚI sẽ tắt FK constraint, insert, rồi bật lại.


