using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class UpdateNoti : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TinTuc",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "TintucID",
                table: "Notifications",
                newName: "TintucId");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TintucID",
                table: "Notifications",
                newName: "IX_Notifications_TintucId");

            migrationBuilder.AlterColumn<int>(
                name: "TintucId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TinTuc_TintucId",
                table: "Notifications",
                column: "TintucId",
                principalTable: "TinTuc",
                principalColumn: "TintucID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TinTuc_TintucId",
                table: "Notifications");

            migrationBuilder.RenameColumn(
                name: "TintucId",
                table: "Notifications",
                newName: "TintucID");

            migrationBuilder.RenameIndex(
                name: "IX_Notifications_TintucId",
                table: "Notifications",
                newName: "IX_Notifications_TintucID");

            migrationBuilder.AlterColumn<int>(
                name: "TintucID",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TinTuc",
                table: "Notifications",
                column: "TintucID",
                principalTable: "TinTuc",
                principalColumn: "TintucID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
