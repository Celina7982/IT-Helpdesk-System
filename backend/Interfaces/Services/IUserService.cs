using IThelpdesk.DTOs.User;
using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Services
{
    public interface IUserService
    {
        //--------------------------------------------------
        // User Lists
        //--------------------------------------------------

        Task<IEnumerable<UserListDto>> GetAllUsersAsync();

        Task<UserDetailsDto?> GetUserByIdAsync(int id);

        Task<User?> GetUserEntityByIdAsync(int id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<IEnumerable<User>> GetTechniciansAsync();

        //--------------------------------------------------
        // CRUD
        //--------------------------------------------------

        Task CreateUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(int id);

        Task ResetPasswordAsync(int id, string newPassword);
    }
}