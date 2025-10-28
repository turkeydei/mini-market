# 🏗️ KIẾN TRÚC DỰ ÁN MINI MARKET

## 📐 Mô hình 3 lớp (3-Tier Architecture)

```
┌─────────────────────────────────────────────┐
│         PRESENTATION LAYER                  │
│              (WebShop)                      │
├─────────────────────────────────────────────┤
│  - Controllers: Xử lý HTTP requests        │
│  - Views: Razor views (.cshtml)            │
│  - ViewModels: Data transfer objects       │
│  - wwwroot: Static files (CSS, JS)         │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌─────────────────────────────────────────────┐
│       BUSINESS LOGIC LAYER                  │
│           (Application)                     │
├─────────────────────────────────────────────┤
│  - Services: Business logic                │
│    • ProductService                        │
│    • OrderService                          │
│    • UserService                           │
│  - Interfaces: Service contracts           │
│  - DTOs: Data transfer objects             │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌─────────────────────────────────────────────┐
│          DATA ACCESS LAYER                  │
│           (Persistence)                     │
├─────────────────────────────────────────────┤
│  - DbContext: EF Core context              │
│  - Repositories: Data access               │
│    • Repository<T>: Generic repo           │
│    • UnitOfWork: Transaction management    │
│  - Migrations: Database schema             │
└────────────────┬────────────────────────────┘
                 │
                 ↓
┌─────────────────────────────────────────────┐
│              DOMAIN LAYER                   │
│              (Domain)                       │
├─────────────────────────────────────────────┤
│  - Entities: Business models               │
│    • User, HangHoa, HoaDon, etc.          │
│  - Value Objects                           │
│  - Domain Events                           │
└─────────────────────────────────────────────┘
```

---

## 📦 CẤU TRÚC THƯ MỤC

```
mini-market/
│
├── Domain/                          # Core Business Entities
│   ├── Entities/
│   │   ├── User.cs                 # User entity
│   │   ├── Loai.cs                 # Category entity
│   │   ├── HangHoa.cs              # Product entity
│   │   ├── HoaDon.cs               # Order entity
│   │   ├── ChiTietHD.cs            # Order detail entity
│   │   └── PaymentTransaction.cs   # Payment entity
│   └── Domain.csproj
│
├── Application/                     # Business Logic Layer
│   ├── Interfaces/
│   │   ├── IRepository.cs          # Generic repository interface
│   │   └── IUnitOfWork.cs          # Unit of work interface
│   ├── Services/
│   │   ├── IProductService.cs      # Product service interface
│   │   ├── ProductService.cs       # Product business logic
│   │   ├── IOrderService.cs        # Order service interface
│   │   └── OrderService.cs         # Order business logic
│   └── Application.csproj
│
├── Persistence/                     # Data Access Layer
│   ├── Repositories/
│   │   ├── Repository.cs           # Generic repository implementation
│   │   └── UnitOfWork.cs           # Unit of work implementation
│   ├── MiniMarketDbContext.cs      # EF Core DbContext
│   ├── SeedData.cs                 # Database seeding
│   ├── Migrations/                 # EF Core migrations
│   └── Persistence.csproj
│
└── WebShop/                        # Presentation Layer
    ├── Controllers/
    │   ├── HomeController.cs       # Home page controller
    │   ├── AuthController.cs       # Authentication controller
    │   ├── CheckoutController.cs   # Checkout controller
    │   └── OrderController.cs      # Order management controller
    ├── Views/
    │   ├── Home/
    │   ├── Auth/
    │   ├── Checkout/
    │   └── Order/
    ├── Models/
    │   ├── LoginViewModel.cs
    │   └── RegisterViewModel.cs
    ├── wwwroot/                    # Static files
    └── WebShop.csproj
```

---

## 🔄 REPOSITORY PATTERN

### IRepository<T> - Generic Repository Interface

```csharp
public interface IRepository<T> where T : class
{
    // Query operations
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);

    // Command operations
    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);

    // Utilities
    Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
    Task<int> CountAsync(Expression<Func<T, bool>> predicate);
}
```

### Repository<T> - Generic Repository Implementation

```csharp
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly MiniMarketDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(MiniMarketDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    // Implementation of all interface methods...
}
```

---

## 🔄 UNIT OF WORK PATTERN

### IUnitOfWork - Quản lý transactions

```csharp
public interface IUnitOfWork : IDisposable
{
    // Repositories for each entity
    IRepository<User> Users { get; }
    IRepository<Loai> Loais { get; }
    IRepository<HangHoa> HangHoas { get; }
    IRepository<HoaDon> HoaDons { get; }
    IRepository<ChiTietHD> ChiTietHDs { get; }
    IRepository<PaymentTransaction> PaymentTransactions { get; }

    // Save all changes in one transaction
    Task<int> SaveAsync();
    int Save();
}
```

### Lợi ích của Unit of Work:

- ✅ Quản lý transactions tập trung
- ✅ Commit tất cả changes cùng lúc
- ✅ Rollback nếu có lỗi
- ✅ Giảm số lần save vào database

---

## 🎯 SERVICE LAYER

### IProductService - Product Business Logic

```csharp
public interface IProductService
{
    Task<IEnumerable<HangHoa>> GetAllProductsAsync();
    Task<IEnumerable<HangHoa>> GetProductsByCategoryAsync(int categoryId);
    Task<HangHoa?> GetProductByIdAsync(int id);
    Task<HangHoa> CreateProductAsync(HangHoa product);
    Task UpdateProductAsync(HangHoa product);
    Task DeleteProductAsync(int id);
    Task<bool> IsProductInStockAsync(int id, int quantity);
}
```

### IOrderService - Order Business Logic

```csharp
public interface IOrderService
{
    Task<HoaDon> CreateOrderAsync(HoaDon order, List<ChiTietHD> orderDetails);
    Task<HoaDon?> GetOrderByIdAsync(int orderId, int userId);
    Task<IEnumerable<HoaDon>> GetOrdersByUserAsync(int userId);
    Task<bool> CancelOrderAsync(int orderId, int userId);
    Task UpdateOrderStatusAsync(int orderId, string status);
    Task<PaymentTransaction> CreatePaymentTransactionAsync(PaymentTransaction transaction);
    Task UpdatePaymentStatusAsync(int transactionId, string status);
}
```

---

## 🔌 DEPENDENCY INJECTION

### Đăng ký services trong Program.cs

```csharp
// Repository Pattern
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Application Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IOrderService, OrderService>();
```

### Sử dụng trong Controller

```csharp
public class HomeController : Controller
{
    private readonly IProductService _productService;

    public HomeController(IProductService productService)
    {
        _productService = productService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);
    }
}
```

---

## 🔄 LUỒNG DỮ LIỆU

### Ví dụ: Lấy danh sách sản phẩm

```
1. User Request
   ↓
2. Controller (HomeController)
   ↓
3. Service (IProductService)
   ↓
4. Unit of Work / Repository
   ↓
5. DbContext → Database
   ↓
6. Entity → Service
   ↓
7. ViewModel → Controller
   ↓
8. View → User
```

### Code flow:

```csharp
// 1. User truy cập /Home/Index
// 2. HomeController nhận request
public async Task<IActionResult> Index()
{
    // 3. Gọi ProductService
    var products = await _productService.GetAllProductsAsync();

    // 7. Trả về View với data
    return View(products);
}

// 4-5. ProductService gọi Repository/DbContext
public async Task<IEnumerable<HangHoa>> GetAllProductsAsync()
{
    return await _context.HangHoas
        .Include(h => h.Loai)
        .ToListAsync();
}
```

---

## 🎯 ƯU ĐIỂM CỦA KIẾN TRÚC NÀY

### ✅ Separation of Concerns

- Mỗi layer có trách nhiệm riêng
- Dễ maintain và test
- Giảm coupling giữa các components

### ✅ Testability

- Dễ dàng mock repositories và services
- Unit test cho từng layer độc lập
- Integration test cho toàn bộ flow

### ✅ Reusability

- Repositories có thể tái sử dụng
- Services chứa business logic chung
- Không duplicate code

### ✅ Maintainability

- Code rõ ràng, dễ đọc
- Thay đổi một layer không ảnh hưởng layer khác
- Dễ mở rộng tính năng mới

### ✅ Scalability

- Dễ thêm entities mới
- Dễ thêm services mới
- Dễ thêm repositories đặc biệt nếu cần

---

## 📚 DESIGN PATTERNS SỬ DỤNG

1. **Repository Pattern**

   - Abstraction layer giữa business logic và data access
   - Centralize data access logic

2. **Unit of Work Pattern**

   - Quản lý transactions
   - Đảm bảo consistency của dữ liệu

3. **Dependency Injection**

   - Loose coupling
   - Dễ test và maintain

4. **Service Layer Pattern**
   - Encapsulate business logic
   - Tách biệt business logic khỏi presentation

---

## 🔍 BEST PRACTICES

### ✅ DO:

- Sử dụng async/await cho database operations
- Inject interfaces, không inject concrete classes
- Xử lý exceptions ở Service layer
- Validate input ở Controller và Service
- Sử dụng DTOs khi trả về data từ API

### ❌ DON'T:

- Không truy cập DbContext trực tiếp từ Controller
- Không để business logic trong Controller
- Không để data access logic trong Service
- Không hardcode connection strings
- Không skip validation

---

## 🚀 MỞ RỘNG TRONG TƯƠNG LAI

1. **CQRS Pattern** - Tách read và write operations
2. **MediatR** - Implement mediator pattern
3. **AutoMapper** - DTO mapping tự động
4. **FluentValidation** - Validation mạnh mẽ hơn
5. **Caching** - Redis hoặc In-Memory cache
6. **Message Queue** - RabbitMQ hoặc Azure Service Bus
7. **Logging** - Serilog structured logging
8. **API Versioning** - Support multiple API versions

---

## 📞 TÓM TẮT

**Kiến trúc hiện tại:**

- ✅ Domain Layer: Entities
- ✅ Application Layer: Services + Interfaces
- ✅ Persistence Layer: Repositories + DbContext
- ✅ Presentation Layer: Controllers + Views

**Patterns đã implement:**

- ✅ Repository Pattern
- ✅ Unit of Work Pattern
- ✅ Service Layer Pattern
- ✅ Dependency Injection

**Kết quả:**

- Clean code
- Easy to test
- Easy to maintain
- Ready to scale
