using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class FixJobCardLabourKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCardLabours_Users_TechnicianId",
                table: "JobCardLabours");

            migrationBuilder.RenameColumn(
                name: "TechnicianId",
                table: "JobCardLabours",
                newName: "CreatedByUserId");

            migrationBuilder.RenameIndex(
                name: "IX_JobCardLabours_TechnicianId",
                table: "JobCardLabours",
                newName: "IX_JobCardLabours_CreatedByUserId");

            migrationBuilder.AlterColumn<string>(
                name: "WorkPerformed",
                table: "JobCardLabours",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(2000)",
                oldMaxLength: 2000);

            migrationBuilder.AlterColumn<decimal>(
                name: "HoursWorked",
                table: "JobCardLabours",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(5,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCardLabours_Users_CreatedByUserId",
                table: "JobCardLabours",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JobCardLabours_Users_CreatedByUserId",
                table: "JobCardLabours");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "JobCardLabours",
                newName: "TechnicianId");

            migrationBuilder.RenameIndex(
                name: "IX_JobCardLabours_CreatedByUserId",
                table: "JobCardLabours",
                newName: "IX_JobCardLabours_TechnicianId");

            migrationBuilder.AlterColumn<string>(
                name: "WorkPerformed",
                table: "JobCardLabours",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<decimal>(
                name: "HoursWorked",
                table: "JobCardLabours",
                type: "decimal(5,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddForeignKey(
                name: "FK_JobCardLabours_Users_TechnicianId",
                table: "JobCardLabours",
                column: "TechnicianId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
