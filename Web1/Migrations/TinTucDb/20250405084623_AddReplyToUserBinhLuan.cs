﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web1.Migrations.TinTucDb
{
    /// <inheritdoc />
    public partial class AddReplyToUserBinhLuan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReplyToUserId",
                table: "BinhLuan",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReplyToUserId",
                table: "BinhLuan");
        }
    }
}
