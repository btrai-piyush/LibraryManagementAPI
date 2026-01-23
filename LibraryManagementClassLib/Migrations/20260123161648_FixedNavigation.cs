using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementClassLib.Migrations
{
    /// <inheritdoc />
    public partial class FixedNavigation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
