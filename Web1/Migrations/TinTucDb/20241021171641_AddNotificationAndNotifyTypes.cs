using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class AddNotificationAndNotifyTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TypeId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "BinhLuan",
                type: "bit",
                nullable: true,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldDefaultValue: true);

            migrationBuilder.CreateTable(
                name: "NotifyTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__NotifyTy__516F03B5067351CF", x => x.TypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_TypeId",
                table: "Notifications",
                column: "TypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotifyTypes",
                table: "Notifications",
                column: "TypeId",
                principalTable: "NotifyTypes",
                principalColumn: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotifyTypes",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotifyTypes");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_TypeId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Notifications");

            migrationBuilder.AlterColumn<bool>(
                name: "TrangThai",
                table: "BinhLuan",
                type: "bit",
                nullable: false,
                defaultValue: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true,
                oldDefaultValue: true);
        }
    }
}
