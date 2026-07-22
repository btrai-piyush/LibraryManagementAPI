using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementClassLib.Migrations
{
    /// <inheritdoc />
    public partial class Semester : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SemesterCode",
                table: "Subjects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SemesterCode",
                table: "Subjects");
        }
    }
}
