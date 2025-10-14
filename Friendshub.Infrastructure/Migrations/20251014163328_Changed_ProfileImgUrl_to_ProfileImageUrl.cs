using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Friendshub.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Changed_ProfileImgUrl_to_ProfileImageUrl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImgUrl",
                table: "Users",
                newName: "ProfileImageUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "Users",
                newName: "ProfileImgUrl");
        }
    }
}
