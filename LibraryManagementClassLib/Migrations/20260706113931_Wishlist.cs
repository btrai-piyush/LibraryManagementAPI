using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementClassLib.Migrations
{
    /// <inheritdoc />
    public partial class Wishlist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_StudentDetail_StudentDetailId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_StudentDetailId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StudentDetailId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Users");

            migrationBuilder.AlterColumn<int>(
                name: "CourseId",
                table: "StudentDetail",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "StudentDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "WishList",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WishList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WishList_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookWishList",
                columns: table => new
                {
                    BooksId = table.Column<int>(type: "int", nullable: false),
                    WishListsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookWishList", x => new { x.BooksId, x.WishListsId });
                    table.ForeignKey(
                        name: "FK_BookWishList_Books_BooksId",
                        column: x => x.BooksId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookWishList_WishList_WishListsId",
                        column: x => x.WishListsId,
                        principalTable: "WishList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudentDetail_CourseId",
                table: "StudentDetail",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentDetail_UserId",
                table: "StudentDetail",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_BookWishList_WishListsId",
                table: "BookWishList",
                column: "WishListsId");

            migrationBuilder.CreateIndex(
                name: "IX_WishList_UserId",
                table: "WishList",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDetail_Courses_CourseId",
                table: "StudentDetail",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentDetail_Users_UserId",
                table: "StudentDetail",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentDetail_Courses_CourseId",
                table: "StudentDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentDetail_Users_UserId",
                table: "StudentDetail");

            migrationBuilder.DropTable(
                name: "BookWishList");

            migrationBuilder.DropTable(
                name: "WishList");

            migrationBuilder.DropIndex(
                name: "IX_StudentDetail_CourseId",
                table: "StudentDetail");

            migrationBuilder.DropIndex(
                name: "IX_StudentDetail_UserId",
                table: "StudentDetail");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "StudentDetail");

            migrationBuilder.AddColumn<int>(
                name: "StudentDetailId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StudentId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CourseId",
                table: "StudentDetail",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Users_StudentDetailId",
                table: "Users",
                column: "StudentDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_StudentDetail_StudentDetailId",
                table: "Users",
                column: "StudentDetailId",
                principalTable: "StudentDetail",
                principalColumn: "Id");
        }
    }
}
