using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class UpdateNotifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_NotifyTypes",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotifyTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Notifica__3214EC0720B93E79",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "Timestamp",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Notifications",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Notifications",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Notifications",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddColumn<bool>(
                name: "IsSystem",
                table: "Notifications",
                type: "bit",
                nullable: true,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Notifications",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetId",
                table: "Notifications",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Notifica__3214EC073F760215",
                table: "Notifications",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "NotificationTypes",
                columns: table => new
                {
                    TypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__516F03B557B4ECEB", x => x.TypeId);
                });

            migrationBuilder.CreateIndex(
                name: "IDX_Notification_CreatedAt",
                table: "Notifications",
                column: "CreatedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IDX_Notification_IsRead",
                table: "Notifications",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IDX_Notification_TargetId",
                table: "Notifications",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "UQ__Notifica__D4E7DFA85869D994",
                table: "NotificationTypes",
                column: "TypeName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Type",
                table: "Notifications",
                column: "TypeId",
                principalTable: "NotificationTypes",
                principalColumn: "TypeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Type",
                table: "Notifications");

            migrationBuilder.DropTable(
                name: "NotificationTypes");

            migrationBuilder.DropPrimaryKey(
                name: "PK__Notifica__3214EC073F760215",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IDX_Notification_CreatedAt",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IDX_Notification_IsRead",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IDX_Notification_TargetId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsSystem",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "TargetId",
                table: "Notifications");

            migrationBuilder.AlterColumn<int>(
                name: "TypeId",
                table: "Notifications",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "Message",
                table: "Notifications",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Notifications",
                type: "int",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("SqlServer:Identity", "1, 1")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Timestamp",
                table: "Notifications",
                type: "datetime",
                nullable: true,
                defaultValueSql: "(getdate())");

            migrationBuilder.AddPrimaryKey(
                name: "PK__Notifica__3214EC0720B93E79",
                table: "Notifications",
                column: "Id");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_NotifyTypes",
                table: "Notifications",
                column: "TypeId",
                principalTable: "NotifyTypes",
                principalColumn: "TypeId");
        }
    }
}
