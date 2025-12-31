# 📚 Giải Thích Chi Tiết Các Features

## 1. ⭐ Product Reviews & Ratings

### 🎯 Mục Đích
Cho phép khách hàng đánh giá và viết review về sản phẩm đã mua, giúp:
- **Tăng engagement**: Khách hàng tương tác nhiều hơn với website
- **Tăng trust**: Reviews giúp khách hàng khác tin tưởng sản phẩm
- **Cải thiện SEO**: Nội dung user-generated tốt cho SEO
- **Tăng conversion**: Sản phẩm có nhiều review tốt bán chạy hơn

### 🔧 Cần Implement

#### Database Schema
```sql
-- Bảng lưu reviews
CREATE TABLE ProductReview (
    MaReview INT PRIMARY KEY IDENTITY(1,1),
    MaHH INT NOT NULL,  -- Sản phẩm
    MaUser INT NOT NULL,  -- Người đánh giá
    Rating INT NOT NULL CHECK (Rating BETWEEN 1 AND 5),  -- 1-5 sao
    Comment NVARCHAR(MAX),  -- Nội dung review
    NgayTao DATETIME DEFAULT GETDATE(),
    IsApproved BIT DEFAULT 0,  -- Admin phê duyệt
    FOREIGN KEY (MaHH) REFERENCES HangHoa(MaHH),
    FOREIGN KEY (MaUser) REFERENCES User(MaUser)
);
```

#### Domain Entity
```csharp
public class ProductReview
{
    public int MaReview { get; set; }
    public int MaHH { get; set; }
    public int MaUser { get; set; }
    public int Rating { get; set; } // 1-5
    public string? Comment { get; set; }
    public DateTime NgayTao { get; set; }
    public bool IsApproved { get; set; }
    
    // Navigation
    public HangHoa? HangHoa { get; set; }
    public User? User { get; set; }
}
```

#### Features Cần Có
1. **Hiển thị reviews trên trang sản phẩm**
   - Average rating (ví dụ: 4.5/5)
   - Số lượng reviews
   - List reviews với pagination
   - Filter: newest, highest, lowest

2. **Form để viết review**
   - Chỉ user đã mua sản phẩm mới được review
   - Rating: 1-5 sao (click để chọn)
   - Textarea cho comment
   - Upload ảnh (optional)

3. **Admin quản lý reviews**
   - Approve/Reject reviews
   - Delete spam reviews
   - View all reviews

#### UI/UX
- ⭐⭐⭐⭐⭐ Star rating display
- "Write a review" button (chỉ hiện nếu đã mua)
- Review cards với avatar, tên, ngày, rating, comment
- "Helpful" button (like review)
- Report spam button

### 📊 Lợi Ích
- **Tăng conversion rate**: 70% người mua đọc reviews trước khi quyết định
- **Tăng trust**: Sản phẩm có 4+ sao bán tốt hơn 30%
- **SEO**: Google ưu tiên hiển thị sản phẩm có reviews

---

## 2. ❤️ Wishlist (Danh Sách Yêu Thích)

### 🎯 Mục Đích
Cho phép khách hàng lưu sản phẩm yêu thích để mua sau:
- **Giữ chân khách hàng**: Họ quay lại website để xem wishlist
- **Tăng sales**: Remind khách hàng về sản phẩm đã lưu
- **Personalization**: Hiểu sở thích khách hàng
- **Cross-sell**: Gợi ý sản phẩm tương tự

### 🔧 Cần Implement

#### Database Schema
```sql
-- Bảng wishlist
CREATE TABLE Wishlist (
    MaWishlist INT PRIMARY KEY IDENTITY(1,1),
    MaUser INT NOT NULL,
    MaHH INT NOT NULL,
    NgayThem DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaUser) REFERENCES User(MaUser),
    FOREIGN KEY (MaHH) REFERENCES HangHoa(MaHH),
    UNIQUE(MaUser, MaHH)  -- Mỗi user chỉ lưu 1 lần
);
```

#### Domain Entity
```csharp
public class Wishlist
{
    public int MaWishlist { get; set; }
    public int MaUser { get; set; }
    public int MaHH { get; set; }
    public DateTime NgayThem { get; set; }
    
    // Navigation
    public User? User { get; set; }
    public HangHoa? HangHoa { get; set; }
}
```

#### Features Cần Có
1. **Add to Wishlist**
   - Heart icon trên product card
   - Click để thêm/xóa khỏi wishlist
   - Toast notification khi thêm thành công
   - Animation khi click (heart fill)

2. **Wishlist Page**
   - Hiển thị tất cả sản phẩm đã lưu
   - Grid layout giống trang sản phẩm
   - Remove từ wishlist
   - Add to cart từ wishlist
   - Share wishlist (optional)

3. **Wishlist Counter**
   - Badge trên header hiển thị số lượng
   - Update real-time khi thêm/xóa

4. **Email Reminders** (optional)
   - Email khi sản phẩm trong wishlist giảm giá
   - Email khi sản phẩm sắp hết hàng

#### UI/UX
- ❤️ Heart icon (outline khi chưa lưu, filled khi đã lưu)
- Wishlist page: `/Wishlist` hoặc `/User/Wishlist`
- Quick actions: Add to cart, Remove, Share
- Empty state: "Bạn chưa có sản phẩm yêu thích nào"

### 📊 Lợi Ích
- **Tăng return visits**: 40% khách hàng quay lại để xem wishlist
- **Tăng conversion**: 25% sản phẩm trong wishlist được mua
- **Email marketing**: Có thể gửi email về sản phẩm đã lưu

---

## 3. 🔐 Password Reset (Đặt Lại Mật Khẩu)

### 🎯 Mục Đích
Cho phép khách hàng đặt lại mật khẩu khi quên:
- **Bảo mật cơ bản**: Feature bắt buộc cho mọi website
- **Giảm support tickets**: Khách hàng tự reset, không cần liên hệ
- **User experience**: Dễ dàng lấy lại tài khoản
- **Security best practice**: Token-based reset an toàn

### 🔧 Cần Implement

#### Database Schema
```sql
-- Bảng lưu reset tokens
CREATE TABLE PasswordResetToken (
    MaToken INT PRIMARY KEY IDENTITY(1,1),
    MaUser INT NOT NULL,
    Token NVARCHAR(255) NOT NULL UNIQUE,  -- Random token
    ExpiresAt DATETIME NOT NULL,  -- Hết hạn sau 1 giờ
    IsUsed BIT DEFAULT 0,  -- Đã sử dụng chưa
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (MaUser) REFERENCES User(MaUser)
);
```

#### Domain Entity
```csharp
public class PasswordResetToken
{
    public int MaToken { get; set; }
    public int MaUser { get; set; }
    public string Token { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public User? User { get; set; }
}
```

#### Flow Hoạt Động

1. **User quên mật khẩu**
   - Click "Quên mật khẩu?" trên trang login
   - Nhập email
   - System gửi email với reset link

2. **Email gửi đi**
   - Link: `https://yoursite.com/Auth/ResetPassword?token=abc123`
   - Token expires sau 1 giờ
   - Email có hướng dẫn

3. **User click link**
   - System verify token (chưa hết hạn, chưa dùng)
   - Hiển thị form nhập mật khẩu mới
   - User nhập mật khẩu mới (2 lần để confirm)

4. **Reset thành công**
   - Hash mật khẩu mới
   - Update database
   - Mark token as used
   - Redirect về login với message success

#### Security Considerations
- **Token phải random và unique**: Dùng GUID hoặc crypto random
- **Token expires**: 1 giờ là hợp lý
- **One-time use**: Token chỉ dùng 1 lần
- **Rate limiting**: Giới hạn số lần request reset (tránh spam)
- **Email validation**: Verify email tồn tại trước khi gửi

#### UI/UX
- **Forgot Password Page**: `/Auth/ForgotPassword`
  - Input email
  - Submit button
  - Message: "Chúng tôi sẽ gửi link reset password vào email của bạn"
  
- **Reset Password Page**: `/Auth/ResetPassword?token=...`
  - Input password mới
  - Input confirm password
  - Submit button
  - Show password strength

- **Email Template**
  - Subject: "Đặt lại mật khẩu Mini Market"
  - Body: Link reset + hướng dẫn + expires time

### 📊 Lợi Ích
- **Giảm support**: 80% user tự reset, không cần hỗ trợ
- **Security**: Token-based an toàn hơn security questions
- **UX**: User-friendly, không cần nhớ câu hỏi bảo mật

---

## 4. 📧 Email Notifications (Thông Báo Đơn Hàng)

### 🎯 Mục Đích
Gửi email tự động khi có sự kiện quan trọng:
- **Thông báo đơn hàng**: Customer biết đơn hàng đã được xác nhận
- **Cập nhật trạng thái**: Thông báo khi order status thay đổi
- **Professional**: Website trông chuyên nghiệp hơn
- **Giảm support**: Customer tự theo dõi, không cần hỏi

### 🔧 Cần Implement

#### Email Service Setup
Cần chọn email service provider:
- **SendGrid** (recommended): Free tier 100 emails/day
- **AWS SES**: Rất rẻ, $0.10 per 1000 emails
- **SMTP Server**: Dùng Gmail/Outlook SMTP (free nhưng có giới hạn)

#### Email Templates Cần Có

1. **Order Confirmation Email**
   - Gửi ngay sau khi đặt hàng thành công
   - Nội dung:
     - Order number
     - Order summary (sản phẩm, số lượng, giá)
     - Total amount
     - Shipping address
     - Payment method
     - Estimated delivery date
     - Link xem order details

2. **Order Status Update Email**
   - Gửi khi admin thay đổi status:
     - "Processing" → "Đơn hàng đang được xử lý"
     - "Shipped" → "Đơn hàng đã được giao cho đơn vị vận chuyển"
     - "Completed" → "Đơn hàng đã được giao thành công"
     - "Cancelled" → "Đơn hàng đã bị hủy"

3. **Payment Confirmation Email**
   - Gửi khi payment thành công
   - Transaction ID
   - Payment method
   - Amount paid

4. **Shipping Notification Email** (optional)
   - Tracking number
   - Link tracking
   - Estimated delivery

#### Implementation

1. **Email Service Interface**
```csharp
public interface IEmailService
{
    Task SendOrderConfirmationAsync(int orderId);
    Task SendOrderStatusUpdateAsync(int orderId, string newStatus);
    Task SendPasswordResetAsync(string email, string token);
    Task SendWelcomeEmailAsync(string email, string name);
}
```

2. **Send Email khi Order Created**
```csharp
// Trong CheckoutController
[HttpPost("CreateOrder")]
public async Task<IActionResult> CreateOrder(...)
{
    // ... create order logic ...
    
    // Send confirmation email
    await _emailService.SendOrderConfirmationAsync(order.MaHD);
    
    return RedirectToAction("Success", "Order");
}
```

3. **Send Email khi Status Changed**
```csharp
// Trong AdminController
[HttpPost("Orders/UpdateStatus/{id}")]
public async Task<IActionResult> UpdateOrderStatus(int id, string status)
{
    // ... update status logic ...
    
    // Send notification email
    await _emailService.SendOrderStatusUpdateAsync(id, status);
    
    return RedirectToAction("OrderDetails", new { id });
}
```

#### Email Template Design
- **HTML Email**: Responsive, đẹp trên mobile
- **Brand colors**: Dùng màu của website
- **Logo**: Logo Mini Market
- **CTA buttons**: "Xem đơn hàng", "Theo dõi vận chuyển"
- **Footer**: Contact info, unsubscribe link

#### Email Content Structure
```
┌─────────────────────────┐
│   Mini Market Logo      │
├─────────────────────────┤
│   Order #12345          │
│   Cảm ơn bạn đã đặt hàng│
├─────────────────────────┤
│   Order Summary         │
│   - Product 1 x 2       │
│   - Product 2 x 1       │
│   Total: 500,000 đ      │
├─────────────────────────┤
│   [Xem đơn hàng] Button │
├─────────────────────────┤
│   Shipping Address      │
│   Payment Method        │
├─────────────────────────┤
│   Footer                │
│   Contact | Unsubscribe │
└─────────────────────────┘
```

### 📊 Lợi Ích
- **Professional**: Website trông chuyên nghiệp
- **Customer satisfaction**: Customer được thông báo kịp thời
- **Giảm support**: 60% câu hỏi về order được giải đáp qua email
- **Marketing**: Có thể thêm promotional content

---

## 📋 Tổng Kết Implementation

### Thứ Tự Ưu Tiên

1. **Password Reset** (1-2 ngày)
   - Quan trọng nhất cho security
   - Dễ implement
   - Impact cao

2. **Email Notifications** (2-3 ngày)
   - Professional
   - Tăng customer satisfaction
   - Cần setup email service

3. **Wishlist** (3-4 ngày)
   - Tăng engagement
   - Giữ chân khách hàng
   - Dễ implement

4. **Product Reviews** (4-5 ngày)
   - Phức tạp nhất
   - Cần admin approval
   - Impact cao nhưng cần thời gian

### Dependencies

- **Email Service**: Cần cho Password Reset và Email Notifications
- **Authentication**: Cần cho Wishlist và Reviews
- **Order System**: Cần cho Reviews (chỉ user đã mua mới review)

### Estimated Time

- **Password Reset**: 1-2 ngày
- **Email Notifications**: 2-3 ngày  
- **Wishlist**: 3-4 ngày
- **Product Reviews**: 4-5 ngày

**Total**: ~2 tuần nếu làm tuần tự, hoặc 1 tuần nếu làm song song

---

## 🚀 Bắt Đầu Từ Đâu?

**Recommendation**: Bắt đầu với **Password Reset** vì:
1. Quan trọng nhất (security)
2. Dễ nhất
3. Có thể reuse email service cho Email Notifications sau

Bạn muốn tôi bắt đầu implement feature nào trước?

