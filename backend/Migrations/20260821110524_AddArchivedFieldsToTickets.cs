using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace IThelpdesk.Migrations
{
    public partial class AddArchivedFieldsToTickets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ArchivedDate",
                table: "Tickets",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ArchivedDate",
                table: "Tickets");
        }
    }
}