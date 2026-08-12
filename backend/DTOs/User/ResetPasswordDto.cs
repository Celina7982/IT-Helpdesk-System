using System.ComponentModel.DataAnnotations;

namespace IThelpdesk.DTOs.User
{
    public class ResetPasswordDto
    {
        [Required]
        [MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
