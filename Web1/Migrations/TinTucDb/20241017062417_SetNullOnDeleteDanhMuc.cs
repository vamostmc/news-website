using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class SetNullOnDeleteDanhMuc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TinTuc__DanhMucI__6383C8BA",
                table: "TinTuc");

            migrationBuilder.AddForeignKey(
                name: "FK__TinTuc__DanhMucI__6383C8BA",
                table: "TinTuc",
                column: "DanhmucID",
                principalTable: "DanhMuc",
                principalColumn: "DanhmucID",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__TinTuc__DanhMucI__6383C8BA",
                table: "TinTuc");

            migrationBuilder.AddForeignKey(
                name: "FK__TinTuc__DanhMucI__6383C8BA",
                table: "TinTuc",
                column: "DanhmucID",
                principalTable: "DanhMuc",
                principalColumn: "DanhmucID");
        }
    }
}
