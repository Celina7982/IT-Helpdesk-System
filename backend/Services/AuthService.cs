// Allows us to use LoginRequestDto and LoginResponseDto.
using IThelpdesk.DTOs.Authentication;

// Allows AuthService to communicate with the UserRepository
// instead of talking directly to the database.
using IThelpdesk.Interfaces.Repositories;

// Allows this class to implement the IAuthService interface.
using IThelpdesk.Interfaces.Services;

// Used to create JWT tokens.
using System.IdentityModel.Tokens.Jwt;

// Used to create claims (information stored inside the token).
using System.Security.Claims;

// Used to convert the JWT key into bytes.
using System.Text;

// Provides classes for signing and validating JWT tokens.
using Microsoft.IdentityModel.Tokens;

// Gives access to values stored in appsettings.json.
using Microsoft.Extensions.Configuration;

namespace IThelpdesk.Services
{
    // AuthService implements the contract defined in IAuthService.
    public class AuthService : IAuthService
    {
        // Repository used to retrieve users from the database.
        private readonly IUserRepository _userRepository;

        // Gives access to values stored in appsettings.json.
        private readonly IConfiguration _configuration;

        // Constructor
        // ASP.NET Core injects these objects automatically.
        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // Called when a user attempts to log in.
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            // Look up the user by email.
            var user = await _userRepository.GetUserByEmailAsync(request.Email);

            // User not found.
            if (user == null)
            {
                return null;
            }

            // Verify the password using BCrypt.
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(
                request.Password,
                user.PasswordHash
            );

            // Password incorrect.
            if (!isPasswordValid)
            {
                return null;
            }

            // Create the information that will be stored in the JWT.
            var claims = new[]
  {
    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),

    // Add this
    new Claim(
        ClaimTypes.Name,
        $"{user.FirstName} {user.LastName}"
    ),

    // Optional but useful
    new Claim(ClaimTypes.GivenName, user.FirstName),

    new Claim(ClaimTypes.Surname, user.LastName),

    new Claim(ClaimTypes.Email, user.Email),

    new Claim(ClaimTypes.Role, user.Role)
};

            // Create the secret signing key from appsettings.json.
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            // Create signing credentials using HMAC SHA256.
            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Create the JWT token.
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            // Convert the JWT object into a string.
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Return the token and expiry date.
            return new LoginResponseDto
            {
                Token = tokenString,
                Expires = token.ValidTo
            };
        }
    }
}