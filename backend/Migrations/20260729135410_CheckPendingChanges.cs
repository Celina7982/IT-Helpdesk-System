using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class CheckPendingChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "JobCardParts");

            migrationBuilder.CreateTable(
                name: "JobCardAudits",
                columns: table => new
                {
                    AuditId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobCardId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobCardAudits", x => x.AuditId);
                    table.ForeignKey(
                        name: "FK_JobCardAudits_JobCards_JobCardId",
                        column: x => x.JobCardId,
                        principalTable: "JobCards",
                        principalColumn: "JobCardId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobCardAudits_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobCardAudits_DateCreated",
                table: "JobCardAudits",
                column: "DateCreated");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardAudits_JobCardId",
                table: "JobCardAudits",
                column: "JobCardId");

            migrationBuilder.CreateIndex(
                name: "IX_JobCardAudits_JobCardId_DateCreated",
                table: "JobCardAudits",
                columns: new[] { "JobCardId", "DateCreated" });

            migrationBuilder.CreateIndex(
                name: "IX_JobCardAudits_UserId",
                table: "JobCardAudits",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobCardAudits");

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "JobCardParts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
