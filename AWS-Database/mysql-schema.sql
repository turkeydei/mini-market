-- MiniShop Database Schema for MySQL
-- Converted from SQL Server to MySQL

CREATE DATABASE IF NOT EXISTS minishop;
USE minishop;

-- Loai (Categories) table
CREATE TABLE Loai (
    MaLoai INT AUTO_INCREMENT PRIMARY KEY,
    TenLoai VARCHAR(255) NOT NULL,
    MoTa TEXT
);

-- TrangThai (Order Status) table
CREATE TABLE TrangThai (
    MaTrangThai INT AUTO_INCREMENT PRIMARY KEY,
    TenTrangThai VARCHAR(255) NOT NULL,
    MoTa VARCHAR(255)
);

-- User table
CREATE TABLE User (
    MaUser INT AUTO_INCREMENT PRIMARY KEY,
    HoTen VARCHAR(255) NOT NULL,
    Email VARCHAR(255) NOT NULL UNIQUE,
    MatKhau VARCHAR(255) NOT NULL,
    DienThoai VARCHAR(20),
    VaiTro INT DEFAULT 0,
    GioiTinh INT,
    NgaySinh DATE,
    DiaChi VARCHAR(255),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
);

-- HangHoa (Products) table
CREATE TABLE HangHoa (
    MaHH INT AUTO_INCREMENT PRIMARY KEY,
    TenHH VARCHAR(255) NOT NULL,
    MaLoai INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    Hinh VARCHAR(255),
    NgaySX DATE,
    GiamGia DECIMAL(5,2) DEFAULT 0,
    SoLanXem INT DEFAULT 0,
    MoTa TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (MaLoai) REFERENCES Loai(MaLoai)
);

-- HoaDon (Orders) table
CREATE TABLE HoaDon (
    MaHD INT AUTO_INCREMENT PRIMARY KEY,
    MaUser INT NOT NULL,
    NgayDat DATETIME DEFAULT CURRENT_TIMESTAMP,
    NgayCan DATETIME,
    NgayGiao DATETIME,
    DiaChiGiao VARCHAR(255) NOT NULL,
    PhiVanChuyen DECIMAL(18,2) NOT NULL,
    MaTrangThai INT NOT NULL,
    GhiChu TEXT,
    SoDienThoai VARCHAR(20),
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    FOREIGN KEY (MaUser) REFERENCES User(MaUser),
    FOREIGN KEY (MaTrangThai) REFERENCES TrangThai(MaTrangThai)
);

-- ChiTietHD (Order Items) table
CREATE TABLE ChiTietHD (
    MaCT INT AUTO_INCREMENT PRIMARY KEY,
    MaHD INT NOT NULL,
    MaHH INT NOT NULL,
    DonGia DECIMAL(18,2) NOT NULL,
    SoLuong INT NOT NULL,
    GiamGia DECIMAL(5,2) DEFAULT 0,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD) ON DELETE CASCADE,
    FOREIGN KEY (MaHH) REFERENCES HangHoa(MaHH) ON DELETE CASCADE
);

-- Indexes for better performance
CREATE INDEX idx_hanghoa_maloai ON HangHoa(MaLoai);
CREATE INDEX idx_hanghoa_tenhh ON HangHoa(TenHH);
CREATE INDEX idx_hoadon_mauser ON HoaDon(MaUser);
CREATE INDEX idx_hoadon_ngaydat ON HoaDon(NgayDat);
CREATE INDEX idx_chitiethd_mahd ON ChiTietHD(MaHD);
CREATE INDEX idx_user_email ON User(Email);

-- Insert sample data
INSERT INTO Loai (TenLoai, MoTa) VALUES
('Điện Thoại', 'Các loại điện thoại di động'),
('Laptop', 'Máy tính xách tay'),
('Phụ Kiện', 'Phụ kiện điện tử'),
('Đồ Gia Dụng', 'Đồ dùng trong gia đình');

INSERT INTO TrangThai (TenTrangThai, MoTa) VALUES
('Chờ Xác Nhận', 'Đơn hàng đang chờ xác nhận'),
('Đã Xác Nhận', 'Đơn hàng đã được xác nhận'),
('Đang Giao', 'Đơn hàng đang được giao'),
('Đã Giao', 'Đơn hàng đã giao thành công'),
('Đã Hủy', 'Đơn hàng đã bị hủy');

INSERT INTO User (HoTen, Email, MatKhau, VaiTro, DienThoai, DiaChi) VALUES
('Admin User', 'admin@example.com', SHA2('admin123', 256), 1, '0123456789', '123 Admin Street'),
('Nguyễn Văn A', 'user1@example.com', SHA2('user123', 256), 0, '0987654321', '456 User Street'),
('Trần Thị B', 'user2@example.com', SHA2('user123', 256), 0, '0369258147', '789 Customer Street');
