# ✅ Admin Panel - Test Checklist

## Trạng thái các bước

### ✅ Bước 4: Quản lý Users
- [x] View danh sách users (`/Admin/Users`)
- [x] Đổi role user (Admin ↔ Customer)
- [x] Xóa user (có kiểm tra không xóa admin cuối cùng)
- [x] DataTables với tìm kiếm và sắp xếp

### ✅ Bước 5: Quản lý Categories
- [x] View danh sách categories (`/Admin/Categories`)
- [x] Tạo category mới (`/Admin/Categories/Create`)
- [x] Sửa category (`/Admin/Categories/Edit/{id}`)
- [x] Xóa category (có kiểm tra không xóa nếu có sản phẩm)

### ✅ Bước 6: Quản lý Products
- [x] View danh sách products (`/Admin/Products`)
- [x] Tìm kiếm và lọc theo category
- [x] Tạo product mới (`/Admin/Products/Create`)
- [x] Upload ảnh sản phẩm
- [x] Sửa product (`/Admin/Products/Edit/{id}`)
- [x] Xóa product (có xóa ảnh kèm theo)

### ✅ Bước 7: Quản lý Orders
- [x] View danh sách orders (`/Admin/Orders`)
- [x] Lọc theo trạng thái (Pending, Processing, Completed, Cancelled)
- [x] Chi tiết đơn hàng (`/Admin/Orders/Details/{id}`)
- [x] Cập nhật trạng thái đơn hàng
- [x] Hủy đơn hàng (có hoàn trả tồn kho)

### ✅ Bước 8: Statistics
- [x] Dashboard với thống kê tổng quan (`/Admin` hoặc `/Admin/Dashboard`)
- [x] Thống kê doanh thu (hôm nay, tháng này, năm nay)
- [x] Thống kê đơn hàng
- [x] Biểu đồ doanh thu theo tháng (Chart.js)
- [x] Top sản phẩm bán chạy
- [x] Đơn hàng gần đây
- [x] Sản phẩm sắp hết hàng

### ✅ Bước 9: Cấu hình routing
- [x] Route prefix `/Admin` trên controller
- [x] Các routes:
  - `/Admin` hoặc `/Admin/Dashboard` → Dashboard
  - `/Admin/Users` → Quản lý users
  - `/Admin/Categories` → Quản lý categories
  - `/Admin/Products` → Quản lý products
  - `/Admin/Orders` → Quản lý orders
  - `/Admin/Statistics` → Thống kê
- [x] Authorization với `[Authorize(Roles = "Admin")]`

### ⏳ Bước 10: Test
Cần test thực tế các chức năng sau:

## Hướng dẫn Test

### 1. Test Authentication & Authorization
```bash
# 1. Đăng nhập với tài khoản Customer (không phải Admin)
# → Truy cập /Admin → Phải redirect về /Auth/AccessDenied

# 2. Đăng nhập với tài khoản Admin
# → Truy cập /Admin → Phải vào được Dashboard
```

### 2. Test Dashboard
- [ ] Kiểm tra các số liệu thống kê hiển thị đúng
- [ ] Kiểm tra đơn hàng gần đây hiển thị
- [ ] Kiểm tra sản phẩm sắp hết hàng hiển thị

### 3. Test Quản lý Users
- [ ] Xem danh sách users
- [ ] Đổi role một user từ Customer → Admin
- [ ] Đổi role một user từ Admin → Customer
- [ ] Thử xóa admin cuối cùng → Phải báo lỗi
- [ ] Xóa một user Customer → Phải thành công

### 4. Test Quản lý Categories
- [ ] Xem danh sách categories
- [ ] Tạo category mới
- [ ] Sửa category
- [ ] Thử xóa category có sản phẩm → Phải báo lỗi
- [ ] Xóa category không có sản phẩm → Phải thành công

### 5. Test Quản lý Products
- [ ] Xem danh sách products
- [ ] Tìm kiếm sản phẩm
- [ ] Lọc theo category
- [ ] Tạo product mới với upload ảnh
- [ ] Kiểm tra ảnh được lưu vào `/wwwroot/images/products/`
- [ ] Sửa product (thay đổi ảnh)
- [ ] Kiểm tra ảnh cũ được xóa
- [ ] Xóa product → Kiểm tra ảnh cũng bị xóa

### 6. Test Quản lý Orders
- [ ] Xem danh sách orders
- [ ] Lọc theo trạng thái
- [ ] Xem chi tiết đơn hàng
- [ ] Cập nhật trạng thái từ Pending → Processing
- [ ] Cập nhật trạng thái từ Processing → Completed
- [ ] Kiểm tra ngày giao được set khi Completed
- [ ] Hủy đơn hàng Pending
- [ ] Kiểm tra tồn kho được hoàn trả
- [ ] Thử hủy đơn hàng Completed → Phải báo lỗi

### 7. Test Statistics
- [ ] Kiểm tra doanh thu hôm nay
- [ ] Kiểm tra doanh thu tháng này
- [ ] Kiểm tra doanh thu năm nay
- [ ] Kiểm tra biểu đồ doanh thu theo tháng hiển thị
- [ ] Kiểm tra top sản phẩm bán chạy

### 8. Test UI/UX
- [ ] Kiểm tra sidebar navigation hoạt động
- [ ] Kiểm tra responsive trên mobile
- [ ] Kiểm tra DataTables hoạt động (tìm kiếm, sắp xếp)
- [ ] Kiểm tra thông báo TempData hiển thị
- [ ] Kiểm tra form validation

## Cách chạy test

1. **Khởi động ứng dụng:**
```bash
cd WebShop
dotnet run
```

2. **Truy cập:**
- Admin Panel: `http://localhost:5000/Admin`
- Login: `http://localhost:5000/Auth/Login`

3. **Tài khoản test:**
- Admin: `admin@minimarket.com` / `Admin@123`
- Customer: `user1@minimarket.com` / `User@123`

## Lưu ý

- Tất cả các chức năng đã được implement
- Code đã compile thành công
- Cần test thực tế để đảm bảo hoạt động đúng
- Nếu có lỗi, kiểm tra:
  - Database connection
  - Seed data đã chạy chưa
  - User có role Admin chưa

