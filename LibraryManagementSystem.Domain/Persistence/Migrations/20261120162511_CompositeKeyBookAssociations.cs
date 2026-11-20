using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Domain.Persistence.Migrations;

/// <inheritdoc />
public partial class _20261120162511_CompositeKeyBookAssociations : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_BookAuthors",
            table: "BookAuthors");

        migrationBuilder.DropIndex(
            name: "IX_BookAuthors_BookId_AuthorId",
            table: "BookAuthors");

        migrationBuilder.DropColumn(
            name: "Id",
            table: "BookTranslators");

        migrationBuilder.DropColumn(
            name: "Id",
            table: "BookAuthors");

        migrationBuilder.AddPrimaryKey(
            name: "PK_BookAuthors",
            table: "BookAuthors",
            columns: new[] { "BookId", "AuthorId" });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropPrimaryKey(
            name: "PK_BookAuthors",
            table: "BookAuthors");

        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "BookTranslators",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddColumn<Guid>(
            name: "Id",
            table: "BookAuthors",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

        migrationBuilder.AddPrimaryKey(
            name: "PK_BookAuthors",
            table: "BookAuthors",
            column: "Id");

        migrationBuilder.CreateIndex(
            name: "IX_BookAuthors_BookId_AuthorId",
            table: "BookAuthors",
            columns: new[] { "BookId", "AuthorId" },
            unique: true);
    }
}
