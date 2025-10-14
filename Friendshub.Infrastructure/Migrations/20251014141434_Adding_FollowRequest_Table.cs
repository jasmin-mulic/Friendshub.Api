using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friendshub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Adding_FollowRequest_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequest_Users_RecieverId",
                table: "FollowRequest");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequest_Users_SenderId",
                table: "FollowRequest");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FollowRequest",
                table: "FollowRequest");

            migrationBuilder.RenameTable(
                name: "FollowRequest",
                newName: "FollowRequests");

            migrationBuilder.RenameIndex(
                name: "IX_FollowRequest_RecieverId",
                table: "FollowRequests",
                newName: "IX_FollowRequests_RecieverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FollowRequests",
                table: "FollowRequests",
                columns: new[] { "SenderId", "RecieverId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequests_Users_RecieverId",
                table: "FollowRequests",
                column: "RecieverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequests_Users_SenderId",
                table: "FollowRequests",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequests_Users_RecieverId",
                table: "FollowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_FollowRequests_Users_SenderId",
                table: "FollowRequests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FollowRequests",
                table: "FollowRequests");

            migrationBuilder.RenameTable(
                name: "FollowRequests",
                newName: "FollowRequest");

            migrationBuilder.RenameIndex(
                name: "IX_FollowRequests_RecieverId",
                table: "FollowRequest",
                newName: "IX_FollowRequest_RecieverId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FollowRequest",
                table: "FollowRequest",
                columns: new[] { "SenderId", "RecieverId" });

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequest_Users_RecieverId",
                table: "FollowRequest",
                column: "RecieverId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FollowRequest_Users_SenderId",
                table: "FollowRequest",
                column: "SenderId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
