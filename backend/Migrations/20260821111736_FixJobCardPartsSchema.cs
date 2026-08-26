using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class FixJobCardPartsSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "JobCardParts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateAdded",
                table: "JobCardParts",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "JobCardParts");

            migrationBuilder.DropColumn(
                name: "DateAdded",
                table: "JobCardParts");
        }
    }
}