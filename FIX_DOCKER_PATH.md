# 🔧 Sửa Lỗi Docker PATH trên macOS

## Vấn đề
Docker Desktop đã được cài đặt và đang chạy, nhưng terminal không tìm thấy lệnh `docker`.

## Giải pháp

### Cách 1: Thêm Docker vào PATH (Đã tự động thêm vào ~/.zshrc)
```bash
export PATH="/usr/local/bin:$PATH"
```

Sau đó khởi động lại terminal hoặc chạy:
```bash
source ~/.zshrc
```

### Cách 2: Chạy script đã được cập nhật
Script `docker-start.sh` đã được cập nhật để tự động tìm Docker ở các vị trí phổ biến:
- `/usr/local/bin/docker`
- `/Applications/Docker.app/Contents/Resources/bin/docker`

Bây giờ bạn có thể chạy:
```bash
./docker-start.sh
```

### Cách 3: Chạy trực tiếp với đường dẫn đầy đủ
```bash
/usr/local/bin/docker --version
/usr/local/bin/docker-compose up -d --build
```

## Kiểm tra Docker

Sau khi thêm vào PATH, kiểm tra:
```bash
docker --version
docker info
```

Nếu vẫn không hoạt động, thử:
```bash
# Kiểm tra Docker có tồn tại không
ls -la /usr/local/bin/docker

# Nếu có, thêm vào PATH tạm thời
export PATH="/usr/local/bin:$PATH"

# Hoặc thêm vĩnh viễn vào ~/.zshrc
echo 'export PATH="/usr/local/bin:$PATH"' >> ~/.zshrc
source ~/.zshrc
```

## Chạy Docker Compose

Sau khi Docker hoạt động, bạn có thể chạy:
```bash
# Cách 1: Dùng script
./docker-start.sh

# Cách 2: Chạy trực tiếp
docker-compose up -d --build

# Hoặc nếu dùng Docker Compose V2
docker compose up -d --build
```

