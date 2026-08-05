using IThelpdesk.DTOs.User;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IThelpdesk.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        // GET: api/User
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        // GET: api/User/5
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);

            if (user == null)
                return NotFound();

            return Ok(user);
        }

        // POST: api/User
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Email = dto.Email,

                    // Service hashes the password
                    PasswordHash = dto.Password,

                    Role = dto.Role,
                    IsActive = dto.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                await _userService.CreateUserAsync(user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }
        // PUT: api/User/{id}
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != dto.UserId)
                return BadRequest();

            try
            {
                var existingUser = await _userService.GetUserEntityByIdAsync(id);

                if (existingUser == null)
                    return NotFound();

                existingUser.FirstName = dto.FirstName;
                existingUser.LastName = dto.LastName;
                existingUser.Email = dto.Email;
                existingUser.Role = dto.Role;
                existingUser.IsActive = dto.IsActive;

                await _userService.UpdateUserAsync(existingUser);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        // DELETE: api/User/{id}
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("technicians")]
        public async Task<IActionResult> GetTechnicians()
        {
            var technicians = await _userService.GetTechniciansAsync();

            return Ok(technicians);
        }
    }
}
