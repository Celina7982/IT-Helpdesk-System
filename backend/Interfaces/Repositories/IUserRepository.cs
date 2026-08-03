using IThelpdesk.Models;

namespace IThelpdesk.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllUsersAsync();

        Task<User?> GetUserByIdAsync(int id);

        Task<User?> GetUserByEmailAsync(string email);

        Task<IEnumerable<User>> GetTechniciansAsync();

        Task AddUserAsync(User user);

        Task UpdateUserAsync(User user);

        Task DeleteUserAsync(User user);

        Task SaveChangesAsync();
    }
}
