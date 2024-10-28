using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class RemoveKhachhangId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Nếu có, xóa ràng buộc khóa ngoại liên quan đến KhachHang trong bảng BinhLuan
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan");

            // Nếu cột KhachhangId tồn tại, xóa nó khỏi bảng BinhLuan
            migrationBuilder.DropColumn(
                name: "KhachhangId",
                table: "BinhLuan");

            // Xóa bảng KhachHang
            migrationBuilder.DropTable(
                name: "KhachHang");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Tạo lại bảng KhachHang nếu cần
            migrationBuilder.CreateTable(
                name: "KhachHang",
                columns: table => new
                {
                    KhachhangID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    // Thêm các cột khác của bảng KhachHang ở đây, ví dụ:
                    TenKhachHang = table.Column<string>(nullable: true),
                    DiaChi = table.Column<string>(nullable: true),
                    SoDienThoai = table.Column<string>(nullable: true),
                    // ... các cột khác
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KhachHang", x => x.KhachhangID);
                });

            // Thêm lại cột KhachhangId vào bảng BinhLuan
            migrationBuilder.AddColumn<int>(
                name: "KhachhangId",
                table: "BinhLuan",
                nullable: true);

            // Thêm lại ràng buộc khóa ngoại cho cột KhachhangId trong bảng BinhLuan
            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan",
                column: "KhachhangId",
                principalTable: "KhachHang",
                principalColumn: "KhachhangID",
                onDelete: ReferentialAction.Restrict);
        }

    }
}
