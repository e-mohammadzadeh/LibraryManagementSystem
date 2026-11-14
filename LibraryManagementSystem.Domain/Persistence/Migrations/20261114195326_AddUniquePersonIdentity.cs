using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Domain.Persistence.Migrations;

/// <inheritdoc />
public partial class _20261114195326_AddUniquePersonIdentity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateIndex(
            name: "IX_Person_Email",
            table: "Person",
            column: "Email",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Person_NationalCode",
            table: "Person",
            column: "NationalCode",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Person_PhoneNumber",
            table: "Person",
            column: "PhoneNumber",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Person_Email",
            table: "Person");

        migrationBuilder.DropIndex(
            name: "IX_Person_NationalCode",
            table: "Person");

        migrationBuilder.DropIndex(
            name: "IX_Person_PhoneNumber",
            table: "Person");
    }
}
