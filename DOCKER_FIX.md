# 🔧 Sửa Lỗi Docker - Port 1433 và Platform

## Vấn đề đã gặp

1. **Port 1433 đã được sử dụng**: Có container SQL Server cũ đang chạy
2. **Platform warning**: Mac M1/M2/M3 (ARM64) cần chỉ định platform cho SQL Server

## Đã sửa

1. ✅ Xóa `version: '3.8'` (obsolete trong Docker Compose V2)
2. ✅ Thêm `platform: linux/amd64` cho SQL Server để chạy với emulation
3. ✅ Dừng và xóa containers cũ

## Chạy lại

```bash
./docker-start.sh
```

Hoặc:

```bash
docker-compose down
docker-compose up -d --build
```

## Lưu ý

- SQL Server sẽ chạy với emulation trên Mac ARM64 (chậm hơn một chút nhưng vẫn hoạt động)
- Nếu muốn nhanh hơn, có thể dùng Azure SQL Edge (nhưng có thể không tương thích 100%)
- Port 1433 sẽ được giải phóng sau khi dừng containers cũ

