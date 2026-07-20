public class CommentDto
{
    public int CommentId { get; set; }

    public string UserName { get; set; } = "";

    public string Comment { get; set; } = "";

    public DateTime CreatedDate { get; set; }
}