using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerAndCompanyToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CompanyName",
                table: "Tickets",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CustomerName",
                table: "Tickets",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyName",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "CustomerName",
                table: "Tickets");
        }
    }
}
