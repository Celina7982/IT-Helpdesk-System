using IThelpdesk.DTOs.User;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface IUserRepository
    {
        //--------------------------------------------------
        // User Lists
        //--------------------------------------------------

        Task<IEnumerable<UserListDto>> GetAllUsersAsync();
        Task<UserDetailsDto?> GetUserByIdAsync(int id);

        Task<User?> GetUserEntityByIdAsync(int id);
        Task<User?> GetUserByEmailAsync(string email);

        Task<IEnumerable<User>> GetTechniciansAsync();
        Task<IEnumerable<User>> GetAdminsAsync();

        //--------------------------------------------------
        // CRUD
        //--------------------------------------------------

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);

        Task SaveChangesAsync();
    }
}