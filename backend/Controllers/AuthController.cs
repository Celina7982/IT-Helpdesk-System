using Microsoft.AspNetCore.Mvc;
using IThelpdesk.DTOs.Authentication;
using IThelpdesk.Interfaces.Services;

namespace IThelpdesk.Controllers
{
    // Marks this class as an API controller.
    [ApiController]

    // Sets the route to:
    // https://localhost:xxxx/api/auth
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        // Reference to the authentication service.
        // The controller asks the service to perform the login.
        private readonly IAuthService _authService;

        // Constructor
        // ASP.NET Core automatically injects AuthService.
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        //
        // This endpoint receives the user's email and password,
        // then returns a JWT if the login succeeds.
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequestDto request)
        {
            // Ask the AuthService to authenticate the user.
            var result = await _authService.LoginAsync(request);

            // Login failed.
            if (result == null)
            {
                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });
            }

            // Login succeeded.
            return Ok(result);
        }
    }
}