using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryInfrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFinesAndExtensionsToBorrowing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Borrowings",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Borrowings",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtensionCount",
                table: "Borrowings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "FineAmount",
                table: "Borrowings",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinePaid",
                table: "Borrowings",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "ExtensionCount",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "FineAmount",
                table: "Borrowings");

            migrationBuilder.DropColumn(
                name: "IsFinePaid",
                table: "Borrowings");
        }
    }
}
