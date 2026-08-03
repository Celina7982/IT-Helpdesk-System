using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class SyncJobCardModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechnicianNotes",
                table: "JobCards");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerSignature",
                table: "JobCards",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompletionNotes",
                table: "JobCards",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AssignedTechnicianId",
                table: "JobCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "JobCards",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaultFound",
                table: "JobCards",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaultReported",
                table: "JobCards",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "SignedDate",
                table: "JobCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WorkPerformed",
                table: "JobCards",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_JobCards_AssignedTechnicianId",
                table: "JobCards",
                column: "AssignedTechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Users_AssignedTechnicianId",
                table: "JobCards",
                column: "AssignedTechnicianId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Users_AssignedTechnicianId",
                table: "JobCards");

            migrationBuilder.DropIndex(
                name: "IX_JobCards_AssignedTechnicianId",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "AssignedTechnicianId",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "FaultFound",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "FaultReported",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "SignedDate",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "WorkPerformed",
                table: "JobCards");

            migrationBuilder.AlterColumn<string>(
                name: "CustomerSignature",
                table: "JobCards",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "CompletionNotes",
                table: "JobCards",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AddColumn<string>(
                name: "TechnicianNotes",
                table: "JobCards",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);
        }
    }
}
