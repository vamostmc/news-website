using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class YourMigrationName2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan");

            migrationBuilder.DropIndex(
                name: "IX_BinhLuan_KhachhangID",
                table: "BinhLuan");

            migrationBuilder.DropColumn(
                name: "KhachhangID",
                table: "BinhLuan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KhachhangID",
                table: "BinhLuan",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_KhachhangID",
                table: "BinhLuan",
                column: "KhachhangID");

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan",
                column: "KhachhangID",
                principalTable: "KhachHang",
                principalColumn: "KhachhangID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
