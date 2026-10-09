using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IThelpdesk.Migrations
{
    /// <inheritdoc />
    public partial class MigrateExistingTicketAssignments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        INSERT INTO TicketAssignments
        (
            TicketId,
            UserId,
            AssignedByUserId,
            AssignedDate
        )
        SELECT
            TicketId,
            AssignedToUserId,
            AssignedToUserId,
            CreatedDate
        FROM Tickets
        WHERE AssignedToUserId IS NOT NULL
          AND NOT EXISTS
          (
              SELECT 1
              FROM TicketAssignments ta
              WHERE ta.TicketId = Tickets.TicketId
                AND ta.UserId = Tickets.AssignedToUserId
          );
    ");
        }
        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Historical assignment rows cannot be safely distinguished
            // from assignments created after this migration.
            // Therefore no automatic delete is performed.
        }
    }
}
