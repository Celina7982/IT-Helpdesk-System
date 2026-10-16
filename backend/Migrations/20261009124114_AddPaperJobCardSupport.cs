using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class AddPaperJobCardSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Tickets_TicketId",
                table: "JobCards");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "JobCards",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "CapturedByUserId",
                table: "JobCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DateCaptured",
                table: "JobCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "JobCardType",
                table: "JobCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaperJobCardDate",
                table: "JobCards",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaperJobCardNumber",
                table: "JobCards",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCards_CapturedByUserId",
                table: "JobCards",
                column: "CapturedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Tickets_TicketId",
                table: "JobCards",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Users_CapturedByUserId",
                table: "JobCards",
                column: "CapturedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Tickets_TicketId",
                table: "JobCards");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCards_Users_CapturedByUserId",
                table: "JobCards");

            migrationBuilder.DropIndex(
                name: "IX_JobCards_CapturedByUserId",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "CapturedByUserId",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "DateCaptured",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "JobCardType",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "PaperJobCardDate",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "PaperJobCardNumber",
                table: "JobCards");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "JobCards",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_JobCards_Tickets_TicketId",
                table: "JobCards",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
