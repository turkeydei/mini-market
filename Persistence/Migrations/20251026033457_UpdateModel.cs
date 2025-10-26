using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHD_HangHoa_HangHoaMaHH",
                table: "ChiTietHD");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHD_HoaDon_HoaDonMaHD",
                table: "ChiTietHD");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_TrangThai_TrangThaiMaTrangThai",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_User_UserMaUser",
                table: "HoaDon");

            migrationBuilder.DropTable(
                name: "PhanQuyen");

            migrationBuilder.DropTable(
                name: "TonKho");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_TrangThaiMaTrangThai",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHD_HangHoaMaHH",
                table: "ChiTietHD");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHD_HoaDonMaHD",
                table: "ChiTietHD");

            migrationBuilder.DropColumn(
                name: "TrangThaiMaTrangThai",
                table: "HoaDon");

            migrationBuilder.DropColumn(
                name: "HangHoaMaHH",
                table: "ChiTietHD");

            migrationBuilder.DropColumn(
                name: "HoaDonMaHD",
                table: "ChiTietHD");

            migrationBuilder.AlterColumn<string>(
                name: "DienThoai",
                table: "User",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserMaUser",
                table: "HoaDon",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "HoaDon",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "PhiVanChuyen",
                table: "HoaDon",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySX",
                table: "HangHoa",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<decimal>(
                name: "GiamGia",
                table: "HangHoa",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "DonGia",
                table: "HangHoa",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(float),
                oldType: "real",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "GiamGia",
                table: "ChiTietHD",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.AlterColumn<decimal>(
                name: "DonGia",
                table: "ChiTietHD",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(float),
                oldType: "real");

            migrationBuilder.CreateTable(
                name: "LichSuGiaoDich",
                columns: table => new
                {
                    MaGiaoDich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHD = table.Column<int>(type: "int", nullable: false),
                    HoaDonMaHD = table.Column<int>(type: "int", nullable: false),
                    Provider = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaGiaoDichVNPAY = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichSuGiaoDich", x => x.MaGiaoDich);
                    table.ForeignKey(
                        name: "FK_LichSuGiaoDich_HoaDon_HoaDonMaHD",
                        column: x => x.HoaDonMaHD,
                        principalTable: "HoaDon",
                        principalColumn: "MaHD",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_MaTrangThai",
                table: "HoaDon",
                column: "MaTrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHD_MaHD",
                table: "ChiTietHD",
                column: "MaHD");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHD_MaHH",
                table: "ChiTietHD",
                column: "MaHH");

            migrationBuilder.CreateIndex(
                name: "IX_LichSuGiaoDich_HoaDonMaHD",
                table: "LichSuGiaoDich",
                column: "HoaDonMaHD");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHD_HangHoa_MaHH",
                table: "ChiTietHD",
                column: "MaHH",
                principalTable: "HangHoa",
                principalColumn: "MaHH",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHD_HoaDon_MaHD",
                table: "ChiTietHD",
                column: "MaHD",
                principalTable: "HoaDon",
                principalColumn: "MaHD",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_TrangThai_MaTrangThai",
                table: "HoaDon",
                column: "MaTrangThai",
                principalTable: "TrangThai",
                principalColumn: "MaTrangThai",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_User_UserMaUser",
                table: "HoaDon",
                column: "UserMaUser",
                principalTable: "User",
                principalColumn: "MaUser",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHD_HangHoa_MaHH",
                table: "ChiTietHD");

            migrationBuilder.DropForeignKey(
                name: "FK_ChiTietHD_HoaDon_MaHD",
                table: "ChiTietHD");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_TrangThai_MaTrangThai",
                table: "HoaDon");

            migrationBuilder.DropForeignKey(
                name: "FK_HoaDon_User_UserMaUser",
                table: "HoaDon");

            migrationBuilder.DropTable(
                name: "LichSuGiaoDich");

            migrationBuilder.DropIndex(
                name: "IX_HoaDon_MaTrangThai",
                table: "HoaDon");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHD_MaHD",
                table: "ChiTietHD");

            migrationBuilder.DropIndex(
                name: "IX_ChiTietHD_MaHH",
                table: "ChiTietHD");

            migrationBuilder.AlterColumn<string>(
                name: "DienThoai",
                table: "User",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "UserMaUser",
                table: "HoaDon",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "SoDienThoai",
                table: "HoaDon",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20,
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "PhiVanChuyen",
                table: "HoaDon",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "TrangThaiMaTrangThai",
                table: "HoaDon",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "NgaySX",
                table: "HangHoa",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<float>(
                name: "GiamGia",
                table: "HangHoa",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<float>(
                name: "DonGia",
                table: "HangHoa",
                type: "real",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<float>(
                name: "GiamGia",
                table: "ChiTietHD",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AlterColumn<float>(
                name: "DonGia",
                table: "ChiTietHD",
                type: "real",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<int>(
                name: "HangHoaMaHH",
                table: "ChiTietHD",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HoaDonMaHD",
                table: "ChiTietHD",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PhanQuyen",
                columns: table => new
                {
                    MaPQ = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaNV = table.Column<int>(type: "int", nullable: false),
                    MaTrang = table.Column<int>(type: "int", nullable: false),
                    Sua = table.Column<bool>(type: "bit", nullable: false),
                    Them = table.Column<bool>(type: "bit", nullable: false),
                    UserMaUser = table.Column<int>(type: "int", nullable: true),
                    Xem = table.Column<bool>(type: "bit", nullable: false),
                    Xoa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhanQuyen", x => x.MaPQ);
                    table.ForeignKey(
                        name: "FK_PhanQuyen_User_UserMaUser",
                        column: x => x.UserMaUser,
                        principalTable: "User",
                        principalColumn: "MaUser");
                });

            migrationBuilder.CreateTable(
                name: "TonKho",
                columns: table => new
                {
                    MaHH = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HangHoaMaHH = table.Column<int>(type: "int", nullable: true),
                    NgayCapNhat = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SoLuongTon = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TonKho", x => x.MaHH);
                    table.ForeignKey(
                        name: "FK_TonKho_HangHoa_HangHoaMaHH",
                        column: x => x.HangHoaMaHH,
                        principalTable: "HangHoa",
                        principalColumn: "MaHH");
                });

            migrationBuilder.CreateIndex(
                name: "IX_HoaDon_TrangThaiMaTrangThai",
                table: "HoaDon",
                column: "TrangThaiMaTrangThai");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHD_HangHoaMaHH",
                table: "ChiTietHD",
                column: "HangHoaMaHH");

            migrationBuilder.CreateIndex(
                name: "IX_ChiTietHD_HoaDonMaHD",
                table: "ChiTietHD",
                column: "HoaDonMaHD");

            migrationBuilder.CreateIndex(
                name: "IX_PhanQuyen_UserMaUser",
                table: "PhanQuyen",
                column: "UserMaUser");

            migrationBuilder.CreateIndex(
                name: "IX_TonKho_HangHoaMaHH",
                table: "TonKho",
                column: "HangHoaMaHH");

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHD_HangHoa_HangHoaMaHH",
                table: "ChiTietHD",
                column: "HangHoaMaHH",
                principalTable: "HangHoa",
                principalColumn: "MaHH",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ChiTietHD_HoaDon_HoaDonMaHD",
                table: "ChiTietHD",
                column: "HoaDonMaHD",
                principalTable: "HoaDon",
                principalColumn: "MaHD",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_TrangThai_TrangThaiMaTrangThai",
                table: "HoaDon",
                column: "TrangThaiMaTrangThai",
                principalTable: "TrangThai",
                principalColumn: "MaTrangThai");

            migrationBuilder.AddForeignKey(
                name: "FK_HoaDon_User_UserMaUser",
                table: "HoaDon",
                column: "UserMaUser",
                principalTable: "User",
                principalColumn: "MaUser");
        }
    }
}
