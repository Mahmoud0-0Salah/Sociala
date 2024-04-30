using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class Editinnames2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Massage_User_SenderId",
                table: "Massage");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Massage",
                table: "Massage");

            migrationBuilder.RenameTable(
                name: "Massage",
                newName: "Message");

            migrationBuilder.RenameIndex(
                name: "IX_Massage_SenderId",
                table: "Message",
                newName: "IX_Message_SenderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Message",
                table: "Message",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Message_User_SenderId",
                table: "Message",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Message_User_SenderId",
                table: "Message");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Message",
                table: "Message");

            migrationBuilder.RenameTable(
                name: "Message",
                newName: "Massage");

            migrationBuilder.RenameIndex(
                name: "IX_Message_SenderId",
                table: "Massage",
                newName: "IX_Massage_SenderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Massage",
                table: "Massage",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Massage_User_SenderId",
                table: "Massage",
                column: "SenderId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
