using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class AddKhachhangIdToBinhLuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan");

            migrationBuilder.RenameColumn(
                name: "KhachhangId",
                table: "BinhLuan",
                newName: "KhachhangID");

            migrationBuilder.RenameIndex(
                name: "IX_BinhLuan_KhachhangId",
                table: "BinhLuan",
                newName: "IX_BinhLuan_KhachhangID");

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_KhachHang",
                table: "BinhLuan",
                column: "KhachhangID",
                principalTable: "KhachHang",
                principalColumn: "KhachhangID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHang",
                table: "BinhLuan");

            migrationBuilder.RenameColumn(
                name: "KhachhangID",
                table: "BinhLuan",
                newName: "KhachhangId");

            migrationBuilder.RenameIndex(
                name: "IX_BinhLuan_KhachhangID",
                table: "BinhLuan",
                newName: "IX_BinhLuan_KhachhangId");

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan",
                column: "KhachhangId",
                principalTable: "KhachHang",
                principalColumn: "KhachhangID");
        }
    }
}
