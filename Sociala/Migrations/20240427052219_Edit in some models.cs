using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class Editinsomemodels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Request",
                table: "Request");

            migrationBuilder.DropIndex(
                name: "IX_Request_RequestingUserId",
                table: "Request");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friend",
                table: "Friend");

            migrationBuilder.DropIndex(
                name: "IX_Friend_RequestingUserId",
                table: "Friend");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Request");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Friend");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Block");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Request",
                table: "Request",
                columns: new[] { "RequestingUserId", "RequestedUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friend",
                table: "Friend",
                columns: new[] { "RequestingUserId", "RequestedUserId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                columns: new[] { "Blocked", "Blocking" });

            migrationBuilder.CreateIndex(
                name: "IX_Block_Blocking",
                table: "Block",
                column: "Blocking");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Request",
                table: "Request");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Friend",
                table: "Friend");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.DropIndex(
                name: "IX_Block_Blocking",
                table: "Block");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Request",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Friend",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Block",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Request",
                table: "Request",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Friend",
                table: "Friend",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                columns: new[] { "Blocking", "Blocked" });

            migrationBuilder.CreateIndex(
                name: "IX_Request_RequestingUserId",
                table: "Request",
                column: "RequestingUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friend_RequestingUserId",
                table: "Friend",
                column: "RequestingUserId");
        }
    }
}
