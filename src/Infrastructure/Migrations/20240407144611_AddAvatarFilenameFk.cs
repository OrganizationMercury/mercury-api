using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAvatarFilenameFk : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Files_AvatarId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AvatarId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AvatarId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Files");

            migrationBuilder.RenameColumn(
                name: "Extension",
                table: "Files",
                newName: "Filename");

            migrationBuilder.AddColumn<string>(
                name: "AvatarFilename",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Filename");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarFilename",
                table: "Users",
                column: "AvatarFilename",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Files_AvatarFilename",
                table: "Users",
                column: "AvatarFilename",
                principalTable: "Files",
                principalColumn: "Filename");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Files_AvatarFilename",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_AvatarFilename",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Files",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "AvatarFilename",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Filename",
                table: "Files",
                newName: "Extension");

            migrationBuilder.AddColumn<Guid>(
                name: "AvatarId",
                table: "Users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Files",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_Files",
                table: "Files",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarId",
                table: "Users",
                column: "AvatarId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Files_AvatarId",
                table: "Users",
                column: "AvatarId",
                principalTable: "Files",
                principalColumn: "Id");
        }
    }
}
