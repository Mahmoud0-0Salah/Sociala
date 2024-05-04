using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sociala.Migrations
{
    /// <inheritdoc />
    public partial class added_notification_actorID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActorId",
                table: "Notification",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Notification_ActorId",
                table: "Notification",
                column: "ActorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notification_User_ActorId",
                table: "Notification",
                column: "ActorId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notification_User_ActorId",
                table: "Notification");

            migrationBuilder.DropIndex(
                name: "IX_Notification_ActorId",
                table: "Notification");

            migrationBuilder.DropColumn(
                name: "ActorId",
                table: "Notification");
        }
    }
}
