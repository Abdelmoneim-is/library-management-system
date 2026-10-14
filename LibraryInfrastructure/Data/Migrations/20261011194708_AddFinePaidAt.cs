using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryInfrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFinePaidAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FinePaidAt",
                table: "Borrowings",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinePaidAt",
                table: "Borrowings");
        }
    }
}
