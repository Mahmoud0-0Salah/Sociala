using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class Editincommentmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Comment",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_UserId",
                table: "Comment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Comment",
                table: "Comment");

            migrationBuilder.DropIndex(
                name: "IX_Comment_UserId",
                table: "Comment");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Comment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Comment",
                table: "Comment",
                columns: new[] { "UserId", "PostId" });
        }
    }
}
