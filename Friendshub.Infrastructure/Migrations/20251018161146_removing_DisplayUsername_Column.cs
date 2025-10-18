using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friendshub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class removing_DisplayUsername_Column : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayUsername",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DisplayUsername",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
