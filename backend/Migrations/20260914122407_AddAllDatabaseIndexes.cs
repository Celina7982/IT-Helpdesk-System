using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class AddAllDatabaseIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Tickets_TicketId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignedToUserId",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedToUserId_IsArchived_CreatedDate",
                table: "Tickets",
                columns: new[] { "AssignedToUserId", "IsArchived", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_Escalation_Status_IsArchived_CreatedDate",
                table: "Tickets",
                columns: new[] { "IsEscalated", "Status", "IsArchived", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IsArchived_ArchivedDate",
                table: "Tickets",
                columns: new[] { "IsArchived", "ArchivedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IsArchived_CreatedDate",
                table: "Tickets",
                columns: new[] { "IsArchived", "CreatedDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId_IsArchived_CreatedDate",
                table: "Tickets",
                columns: new[] { "UserId", "IsArchived", "CreatedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_Email",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_AssignedToUserId_IsArchived_CreatedDate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_Escalation_Status_IsArchived_CreatedDate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_IsArchived_ArchivedDate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_IsArchived_CreatedDate",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_UserId_IsArchived_CreatedDate",
                table: "Tickets");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_AssignedToUserId",
                table: "Tickets",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_UserId",
                table: "Tickets",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Tickets_TicketId",
                table: "Notifications",
                column: "TicketId",
                principalTable: "Tickets",
                principalColumn: "TicketId");
        }
    }
}
