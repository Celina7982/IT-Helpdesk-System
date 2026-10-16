using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class AddPaperJobCardWorkAndParts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnSite",
                table: "JobCards",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsTravel",
                table: "JobCards",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ServiceType",
                table: "JobCards",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TimeIn",
                table: "JobCards",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "TimeOut",
                table: "JobCards",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CapturedByUserId",
                table: "JobCardParts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaperPartEntry",
                table: "JobCardParts",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ResponsibleTechnicianId",
                table: "JobCardParts",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CapturedByUserId",
                table: "JobCardLabours",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaperWorkEntry",
                table: "JobCardLabours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "JobCardTechnicians",
                columns: table => new
                {
                    JobCardTechnicianId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobCardId = table.Column<int>(type: "int", nullable: false),
                    TechnicianId = table.Column<int>(type: "int", nullable: false),
                    IsLeadTechnician = table.Column<bool>(type: "bit", nullable: false),
                    AssignedByUserId = table.Column<int>(type: "int", nullable: false),
                    DateAssigned = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCardTechnicians", x => x.JobCardTechnicianId);
                    table.ForeignKey(
                        name: "FK_JobCardTechnicians_JobCards_JobCardId",
                        column: x => x.JobCardId,
                        principalTable: "JobCards",
                        principalColumn: "JobCardId");
                    table.ForeignKey(
                        name: "FK_JobCardTechnicians_Users_AssignedByUserId",
                        column: x => x.AssignedByUserId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                    table.ForeignKey(
                        name: "FK_JobCardTechnicians_Users_TechnicianId",
                        column: x => x.TechnicianId,
                        principalTable: "Users",
                        principalColumn: "UserId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCardParts_CapturedByUserId",
                table: "JobCardParts",
                column: "CapturedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardParts_ResponsibleTechnicianId",
                table: "JobCardParts",
                column: "ResponsibleTechnicianId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardLabours_CapturedByUserId",
                table: "JobCardLabours",
                column: "CapturedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardTechnicians_AssignedByUserId",
                table: "JobCardTechnicians",
                column: "AssignedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardTechnicians_JobCardId",
                table: "JobCardTechnicians",
                column: "JobCardId",
                unique: true,
                filter: "[IsLeadTechnician] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardTechnicians_JobCardId_TechnicianId",
                table: "JobCardTechnicians",
                columns: new[] { "JobCardId", "TechnicianId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_JobCardTechnicians_TechnicianId",
                table: "JobCardTechnicians",
                column: "TechnicianId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCardLabours_Users_CapturedByUserId",
                table: "JobCardLabours",
                column: "CapturedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCardParts_Users_CapturedByUserId",
                table: "JobCardParts",
                column: "CapturedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCardParts_Users_ResponsibleTechnicianId",
                table: "JobCardParts",
                column: "ResponsibleTechnicianId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCardLabours_Users_CapturedByUserId",
                table: "JobCardLabours");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCardParts_Users_CapturedByUserId",
                table: "JobCardParts");

            migrationBuilder.DropForeignKey(
                name: "FK_JobCardParts_Users_ResponsibleTechnicianId",
                table: "JobCardParts");

            migrationBuilder.DropTable(
                name: "JobCardTechnicians");

            migrationBuilder.DropIndex(
                name: "IX_JobCardParts_CapturedByUserId",
                table: "JobCardParts");

            migrationBuilder.DropIndex(
                name: "IX_JobCardParts_ResponsibleTechnicianId",
                table: "JobCardParts");

            migrationBuilder.DropIndex(
                name: "IX_JobCardLabours_CapturedByUserId",
                table: "JobCardLabours");

            migrationBuilder.DropColumn(
                name: "IsOnSite",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "IsTravel",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "ServiceType",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "TimeIn",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "TimeOut",
                table: "JobCards");

            migrationBuilder.DropColumn(
                name: "CapturedByUserId",
                table: "JobCardParts");

            migrationBuilder.DropColumn(
                name: "IsPaperPartEntry",
                table: "JobCardParts");

            migrationBuilder.DropColumn(
                name: "ResponsibleTechnicianId",
                table: "JobCardParts");

            migrationBuilder.DropColumn(
                name: "CapturedByUserId",
                table: "JobCardLabours");

            migrationBuilder.DropColumn(
                name: "IsPaperWorkEntry",
                table: "JobCardLabours");
        }
    }
}
