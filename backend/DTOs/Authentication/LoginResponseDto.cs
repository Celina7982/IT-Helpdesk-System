namespace IThelpdesk.DTOs.Authentication
{

    // class to represent the response of a login operation
    //data transfer object (DTO) is a design pattern used to transfer data between software application subsystems

    //This represents the data the client sends when attempting to log in.
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;

        public DateTime Expires { get; set; }
    }
}
