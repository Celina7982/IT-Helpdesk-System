using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Interfaces.Services;
using IThelpdesk.Models;
using IThelpdesk.DTOs.User;

namespace IThelpdesk.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllUsersAsync();
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(int id)
        {
            return await _userRepository.GetUserByIdAsync(id);
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _userRepository.GetUserEntityByIdAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetUserByEmailAsync(email);
        }

        public async Task CreateUserAsync(User user)
        {
            // Check if the email already exists
            var existingUser = await _userRepository.GetUserByEmailAsync(user.Email);

            if (existingUser != null)
            {
                throw new Exception("A user with this email already exists.");
            }

            // Hash the password before saving it
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

            // Save the new user
            await _userRepository.AddUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            await _userRepository.DeleteUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }


        public async Task ResetPasswordAsync(int id, string newPassword)
        {
            var user = await _userRepository.GetUserEntityByIdAsync(id);

            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Hash the new password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);

            await _userRepository.UpdateUserAsync(user);
            await _userRepository.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetTechniciansAsync()
        {
            return await _userRepository.GetTechniciansAsync();
        }
    }
}
