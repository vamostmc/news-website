using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class AddNotificationsTinTuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TintucID",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TintucID",
                table: "Notifications",
                column: "TintucID");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_TinTuc",
                table: "Notifications",
                column: "TintucID",
                principalTable: "TinTuc",
                principalColumn: "TintucID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_TinTuc",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TintucID",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TintucID",
                table: "Notifications");
        }
    }
}
