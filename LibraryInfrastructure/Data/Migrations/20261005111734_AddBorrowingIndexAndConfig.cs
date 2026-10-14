using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryInfrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowingIndexAndConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Borrowings",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Borrowings_UserId_ReturnDate",
                table: "Borrowings",
                columns: new[] { "UserId", "ReturnDate" });

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings");

            migrationBuilder.DropIndex(
                name: "IX_Borrowings_UserId_ReturnDate",
                table: "Borrowings");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Borrowings",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Borrowings_Books_BookId",
                table: "Borrowings",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
