using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friendshub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Notification_Update : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isOpened",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isOpened",
                table: "Notifications");
        }
    }
}
