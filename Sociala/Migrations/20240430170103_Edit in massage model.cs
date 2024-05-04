using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class Editinmassagemodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Massage",
                table: "Massage");

            migrationBuilder.AlterColumn<string>(
                name: "ResverId",
                table: "Massage",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Massage",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Massage",
                table: "Massage",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Massage_SenderId",
                table: "Massage",
                column: "SenderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Massage",
                table: "Massage");

            migrationBuilder.DropIndex(
                name: "IX_Massage_SenderId",
                table: "Massage");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Massage");

            migrationBuilder.AlterColumn<string>(
                name: "ResverId",
                table: "Massage",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Massage",
                table: "Massage",
                columns: new[] { "SenderId", "ResverId" });
        }
    }
}
