using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class AddSharePostTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SharePost",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SharePost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SharePost_Post_PostId",
                        column: x => x.PostId,
                        principalTable: "Post",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SharePost_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SharePost_PostId",
                table: "SharePost",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_SharePost_UserId",
                table: "SharePost",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SharePost");
        }
    }
}
