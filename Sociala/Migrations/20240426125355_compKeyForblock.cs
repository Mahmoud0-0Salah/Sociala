using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class compKeyForblock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Block",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                columns: new[] { "Id", "Blocking", "Blocked" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Block",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                column: "Id");
        }
    }
}
