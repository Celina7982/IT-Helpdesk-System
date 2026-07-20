using System.ComponentModel.DataAnnotations;

public class CreateCommentDto
{
    [Required]
    public string Comment { get; set; } = string.Empty;
}