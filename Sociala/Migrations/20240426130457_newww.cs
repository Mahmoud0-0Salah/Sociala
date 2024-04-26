using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class newww : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.DropIndex(
                name: "IX_Block_Blocking",
                table: "Block");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                columns: new[] { "Blocking", "Blocked" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Block",
                table: "Block");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Block",
                table: "Block",
                columns: new[] { "Id", "Blocking", "Blocked" });

            migrationBuilder.CreateIndex(
                name: "IX_Block_Blocking",
                table: "Block",
                column: "Blocking");
        }
    }
}
