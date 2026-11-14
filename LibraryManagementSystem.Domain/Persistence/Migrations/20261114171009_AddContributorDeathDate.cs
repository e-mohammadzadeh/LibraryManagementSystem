using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagementSystem.Domain.Persistence.Migrations;

/// <inheritdoc />
public partial class _20261114171009_AddContributorDeathDate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<DateOnly>(
            name: "DeathDate",
            table: "Person",
            type: "date",
            nullable: true);

        migrationBuilder.AlterColumn<Guid>(
            name: "PerformedByUserId",
            table: "AuditLogs",
            type: "uniqueidentifier",
            nullable: true,
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier");

        migrationBuilder.AddForeignKey(
            name: "FK_AuditLogs_Person_PerformedByUserId",
            table: "AuditLogs",
            column: "PerformedByUserId",
            principalTable: "Person",
            principalColumn: "Id",
            onDelete: ReferentialAction.SetNull);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_AuditLogs_Person_PerformedByUserId",
            table: "AuditLogs");

        migrationBuilder.DropColumn(
            name: "DeathDate",
            table: "Person");

        migrationBuilder.AlterColumn<Guid>(
            name: "PerformedByUserId",
            table: "AuditLogs",
            type: "uniqueidentifier",
            nullable: false,
            defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
            oldClrType: typeof(Guid),
            oldType: "uniqueidentifier",
            oldNullable: true);
    }
}
