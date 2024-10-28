using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class AddTrangThaiToDanhMuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TinTuc_TintucId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TintucId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SoLuongComment",
                table: "TinTuc");

            migrationBuilder.DropColumn(
                name: "TintucId",
                table: "Notifications");

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "TinTuc",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AlterColumn<int>(
                name: "SoLuongTinTuc",
                table: "DanhMuc",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldDefaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "TrangThai",
                table: "DanhMuc",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "TinTuc");

            migrationBuilder.DropColumn(
                name: "TrangThai",
                table: "DanhMuc");

            migrationBuilder.AddColumn<int>(
                name: "SoLuongComment",
                table: "TinTuc",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TintucId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "SoLuongTinTuc",
                table: "DanhMuc",
                type: "int",
                nullable: true,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TintucId",
                table: "Notifications",
                column: "TintucId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TinTuc_TintucId",
                table: "Notifications",
                column: "TintucId",
                principalTable: "TinTuc",
                principalColumn: "TintucID");
        }
    }
}
