namespace IThelpdesk.DTOs
{
    public class CreateCommentDto
    {
        public string Message { get; set; } = string.Empty;
    }

    public class CommentResponseDto
    {
        public int CommentId { get; set; }
        public int TicketId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class UpdateCommentDto
    {
        public string Message { get; set; } = string.Empty;
    }
}