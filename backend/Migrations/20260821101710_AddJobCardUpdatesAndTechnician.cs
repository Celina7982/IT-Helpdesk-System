using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    public partial class AddJobCardUpdatesAndTechnician : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TechnicianId",
                table: "JobCards",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TechnicianId",
                table: "JobCards");
        }
    }
}