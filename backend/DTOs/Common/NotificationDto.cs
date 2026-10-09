using System;

namespace IThelpdesk.DTOs.Common
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }

        public int? TicketId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; }

        public DateTime DateCreated { get; set; }
    }
}