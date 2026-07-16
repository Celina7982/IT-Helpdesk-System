using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketAssignmentAndEscalation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AssignedToUserId",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EscalationReason",
                table: "Tickets",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEscalated",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AssignedToUserId",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "EscalationReason",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IsEscalated",
                table: "Tickets");
        }
    }
}
