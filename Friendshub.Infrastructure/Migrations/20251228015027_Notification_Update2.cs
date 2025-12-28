using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friendshub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Notification_Update2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isRead",
                table: "Notifications",
                newName: "IsRead");

            migrationBuilder.RenameColumn(
                name: "isOpened",
                table: "Notifications",
                newName: "IsOpened");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsRead",
                table: "Notifications",
                newName: "isRead");

            migrationBuilder.RenameColumn(
                name: "IsOpened",
                table: "Notifications",
                newName: "isOpened");
        }
    }
}
