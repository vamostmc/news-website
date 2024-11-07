using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class UpdateBinhLuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Likes",
                table: "BinhLuan",
                type: "int",
                nullable: true,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "BinhLuan",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BinhLuan_ParentId",
                table: "BinhLuan",
                column: "ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_BinhLuan_ParentID",
                table: "BinhLuan",
                column: "ParentId",
                principalTable: "BinhLuan",
                principalColumn: "BinhluanID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BinhLuan_ParentID",
                table: "BinhLuan");

            migrationBuilder.DropIndex(
                name: "IX_BinhLuan_ParentId",
                table: "BinhLuan");

            migrationBuilder.DropColumn(
                name: "Likes",
                table: "BinhLuan");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "BinhLuan");
        }
    }
}
