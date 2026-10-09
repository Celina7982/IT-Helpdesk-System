namespace IThelpdesk.DTOs
{
    public class TicketAssigneeDto
    {
        public int UserId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public DateTime AssignedDate { get; set; }

        public int AssignedByUserId { get; set; }

        public string AssignedByName { get; set; } = string.Empty;
    }
}
