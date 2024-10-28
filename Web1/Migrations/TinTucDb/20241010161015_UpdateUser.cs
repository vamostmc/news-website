using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class UpdateUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint if it exists
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_KhachHang_KhachhangId",
                table: "BinhLuan");

            // Remove the KhachhangId index if it exists
            migrationBuilder.DropIndex(
                name: "IX_BinhLuan_KhachhangId",
                table: "BinhLuan");

            // Drop the KhachhangId column from BinhLuan table
            migrationBuilder.DropColumn(
                name: "KhachhangId",
                table: "BinhLuan");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KhachhangId",
                table: "BinhLuan",
                type: "int",
                nullable: true);

            // Recreate the index for KhachhangId
            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_KhachhangId",
                table: "BinhLuan",
                column: "KhachhangId");

            // Recreate the foreign key constraint
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
