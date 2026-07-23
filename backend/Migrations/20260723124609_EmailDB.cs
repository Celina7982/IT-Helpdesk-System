using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class EmailDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId1",
                table: "TicketComments");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_TicketId1",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "TicketId1",
                table: "TicketComments");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketComments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "TicketNum",
                table: "TicketComments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketNum",
                table: "TicketComments",
                column: "TicketNum");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketNum",
                table: "TicketComments",
                column: "TicketNum",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments");

            migrationBuilder.DropForeignKey(
                name: "FK_TicketComments_Tickets_TicketNum",
                table: "TicketComments");

            migrationBuilder.DropIndex(
                name: "IX_TicketComments_TicketNum",
                table: "TicketComments");

            migrationBuilder.DropColumn(
                name: "TicketNum",
                table: "TicketComments");

            migrationBuilder.AlterColumn<int>(
                name: "TicketId",
                table: "TicketComments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TicketId1",
                table: "TicketComments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TicketComments_TicketId1",
                table: "TicketComments",
                column: "TicketId1");

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId",
                table: "TicketComments",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TicketComments_Tickets_TicketId1",
                table: "TicketComments",
                column: "TicketId1",
                principalTable: "Tickets",
                principalColumn: "TicketId");
        }
    }
}
