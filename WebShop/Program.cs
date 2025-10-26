using Microsoft.EntityFrameworkCore;
using Persistence;
using Application.Features.Interface;
using Application.Features.Services;
using Persistence.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add DbContext
builder.Services.AddDbContext<MiniMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Repositories
builder.Services.AddScoped<Application.Features.Interface.IRepositories.IHangHoaRepository, HangHoaRepository>();
builder.Services.AddScoped<Application.Features.Interface.IRepositories.IUserRepository, UserRepository>();
builder.Services.AddScoped<Application.Features.Interface.IRepositories.IHoaDonRepository, HoaDonRepository>();

// Register Services
builder.Services.AddScoped<IHangHoaService, HangHoaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IHoaDonService, HoaDonService>();
builder.Services.AddScoped<IAuthService, AuthenticationService>();

// Add HttpContextAccessor for AuthenticationService
builder.Services.AddHttpContextAccessor();

// Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Authentication/Login";
        options.LogoutPath = "/Authentication/Logout";
        options.AccessDeniedPath = "/Authentication/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
        options.SlidingExpiration = true;
    });

// Add Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<MiniMarketDbContext>();
    await Persistence.SeedData.SeedAsync(context);
}

app.Run();
