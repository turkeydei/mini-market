# 📥 Hướng dẫn cài đặt .NET SDK

## macOS

### Cách 1: Sử dụng Homebrew (Khuyến nghị)

```bash
# Cài đặt Homebrew nếu chưa có
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Cài đặt .NET SDK
brew install --cask dotnet-sdk

# Kiểm tra
dotnet --version
```

### Cách 2: Download trực tiếp

1. Truy cập: https://dotnet.microsoft.com/download
2. Chọn **Download .NET SDK** (phiên bản 6.0 hoặc cao hơn)
3. Tải file `.pkg` cho macOS
4. Mở file và làm theo hướng dẫn cài đặt
5. Khởi động lại Terminal
6. Kiểm tra: `dotnet --version`

---

## Windows

### Cách 1: Download và cài đặt

1. Truy cập: https://dotnet.microsoft.com/download
2. Chọn **Download .NET SDK** (phiên bản 6.0 hoặc cao hơn)
3. Tải file `.exe` cho Windows
4. Chạy file cài đặt
5. Khởi động lại Command Prompt hoặc PowerShell
6. Kiểm tra: `dotnet --version`

### Cách 2: Sử dụng Winget

```powershell
# Cài đặt qua Winget (Windows 10/11)
winget install Microsoft.DotNet.SDK.8

# Kiểm tra
dotnet --version
```

---

## Linux (Ubuntu/Debian)

```bash
# Thêm Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Cập nhật package list
sudo apt-get update

# Cài đặt .NET SDK
sudo apt-get install -y dotnet-sdk-8.0

# Kiểm tra
dotnet --version
```

---

## Kiểm tra cài đặt thành công

Mở Terminal/Command Prompt và chạy:

```bash
dotnet --version
```

Kết quả hiển thị phiên bản .NET SDK (ví dụ: `8.0.100`)

---

## Cài đặt EF Core Tools

Sau khi cài đặt .NET SDK, cài đặt Entity Framework Core Tools:

```bash
dotnet tool install --global dotnet-ef

# Kiểm tra
dotnet ef --version
```

---

## Cài đặt SQL Server

### macOS

**Option 1: Docker (Khuyến nghị)**

```bash
# Pull SQL Server Docker image
docker pull mcr.microsoft.com/mssql/server:2022-latest

# Run SQL Server container
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Password" \
   -p 1433:1433 --name sqlserver \
   -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string trong appsettings.json:
# Server=localhost,1433;Database=MiniMarketDB;User Id=sa;Password=YourStrong@Password;TrustServerCertificate=True
```

**Option 2: Azure SQL (Cloud)**

- Tạo Azure SQL Database miễn phí tại: https://azure.microsoft.com/free/

### Windows

**SQL Server Express (Miễn phí)**

1. Tải từ: https://www.microsoft.com/sql-server/sql-server-downloads
2. Chọn **Express** edition
3. Cài đặt với tên instance mặc định
4. Sử dụng Windows Authentication

**SQL Server LocalDB (Nhẹ hơn)**

```bash
# Cài đặt qua Visual Studio Installer
# Hoặc download SQL Server LocalDB
```

### Linux (Ubuntu)

```bash
# Thêm Microsoft repository
wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -
sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/mssql-server-2022.list)"

# Cài đặt SQL Server
sudo apt-get update
sudo apt-get install -y mssql-server

# Cấu hình
sudo /opt/mssql/bin/mssql-conf setup

# Kiểm tra
systemctl status mssql-server
```

---

## Troubleshooting

### macOS: "dotnet command not found" sau khi cài

```bash
# Thêm vào ~/.zshrc hoặc ~/.bash_profile
export PATH="$PATH:/usr/local/share/dotnet"

# Reload shell
source ~/.zshrc  # hoặc source ~/.bash_profile
```

### Windows: "dotnet is not recognized"

1. Khởi động lại Command Prompt/PowerShell
2. Kiểm tra PATH environment variable
3. Cài đặt lại .NET SDK

### Linux: Permission denied khi cài EF Tools

```bash
# Thêm vào ~/.bashrc
export PATH="$PATH:$HOME/.dotnet/tools"

# Reload
source ~/.bashrc
```

---

## Kiểm tra tổng thể

Sau khi cài đặt tất cả, chạy các lệnh sau để kiểm tra:

```bash
# 1. .NET SDK
dotnet --version
# Output: 8.0.xxx hoặc cao hơn

# 2. EF Core Tools
dotnet ef --version
# Output: Entity Framework Core .NET Command-line Tools x.x.x

# 3. SQL Server (Windows với Windows Auth)
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# 4. SQL Server (Docker)
docker ps | grep sqlserver
# Hoặc
sqlcmd -S localhost,1433 -U sa -P "YourStrong@Password" -Q "SELECT @@VERSION"
```

---

## Sẵn sàng!

Sau khi cài đặt xong:

1. ✅ .NET SDK
2. ✅ EF Core Tools
3. ✅ SQL Server

Quay lại `QUICKSTART.md` và chạy `./setup.sh` (macOS/Linux) hoặc `.\setup.ps1` (Windows)

🚀 **Happy Coding!**
