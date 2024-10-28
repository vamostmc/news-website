using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class AddNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_AspNetUsers",
                table: "BinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_TinTuc_TintucId",
                table: "BinhLuan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BinhLuan",
                table: "BinhLuan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachHang",
                table: "KhachHang");

            migrationBuilder.RenameTable(
                name: "KhachHang",
                newName: "KhachHangs");

            migrationBuilder.RenameColumn(
                name: "UserID",
                table: "BinhLuan",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TintucId",
                table: "BinhLuan",
                newName: "TintucID");

            migrationBuilder.RenameColumn(
                name: "BinhluanId",
                table: "BinhLuan",
                newName: "BinhluanID");

            migrationBuilder.RenameIndex(
                name: "IX_BinhLuan_UserID",
                table: "BinhLuan",
                newName: "IX_BinhLuan_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BinhLuan_TintucId",
                table: "BinhLuan",
                newName: "IX_BinhLuan_TintucID");

            migrationBuilder.RenameColumn(
                name: "KhachhangID",
                table: "KhachHangs",
                newName: "KhachhangId");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "BinhLuan",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "KhachHangs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "TenKhachHang",
                table: "KhachHangs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK__BinhLuan__7B6CA14E405369F7",
                table: "BinhLuan",
                column: "BinhluanID");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachHangs",
                table: "KhachHangs",
                column: "KhachhangId");

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Message = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__3214EC0720B93E79", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_AspNetUsers_UserId",
                table: "BinhLuan",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK__BinhLuan__Tintuc__73BA3083",
                table: "BinhLuan",
                column: "TintucID",
                principalTable: "TinTuc",
                principalColumn: "TintucID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_AspNetUsers_UserId",
                table: "BinhLuan");

            migrationBuilder.DropForeignKey(
                name: "FK__BinhLuan__Tintuc__73BA3083",
                table: "BinhLuan");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropPrimaryKey(
                name: "PK__BinhLuan__7B6CA14E405369F7",
                table: "BinhLuan");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KhachHangs",
                table: "KhachHangs");

            migrationBuilder.RenameTable(
                name: "KhachHangs",
                newName: "KhachHang");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "BinhLuan",
                newName: "UserID");

            migrationBuilder.RenameColumn(
                name: "TintucID",
                table: "BinhLuan",
                newName: "TintucId");

            migrationBuilder.RenameColumn(
                name: "BinhluanID",
                table: "BinhLuan",
                newName: "BinhluanId");

            migrationBuilder.RenameIndex(
                name: "IX_BinhLuan_UserId",
                table: "BinhLuan",
                newName: "IX_BinhLuan_UserID");

            migrationBuilder.RenameIndex(
                name: "IX_BinhLuan_TintucID",
                table: "BinhLuan",
                newName: "IX_BinhLuan_TintucId");

            migrationBuilder.RenameColumn(
                name: "KhachhangId",
                table: "KhachHang",
                newName: "KhachhangID");

            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "BinhLuan",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VaiTro",
                table: "KhachHang",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TenKhachHang",
                table: "KhachHang",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BinhLuan",
                table: "BinhLuan",
                column: "BinhluanId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_KhachHang",
                table: "KhachHang",
                column: "KhachhangID");

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_AspNetUsers",
                table: "BinhLuan",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_TinTuc_TintucId",
                table: "BinhLuan",
                column: "TintucId",
                principalTable: "TinTuc",
                principalColumn: "TintucID");
        }
    }
}
