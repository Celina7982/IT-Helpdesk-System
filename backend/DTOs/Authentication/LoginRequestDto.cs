namespace IThelpdesk.DTOs.Authentication
{
    public class LoginRequestDto //This class represents the data a client sends when attempting to log in.
    {
        public string Email { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}