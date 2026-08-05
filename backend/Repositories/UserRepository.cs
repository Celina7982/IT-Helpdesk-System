using IThelpdesk.Data;
using IThelpdesk.DTOs.User;
using IThelpdesk.Interfaces.Repositories;
using IThelpdesk.Models;
using Microsoft.EntityFrameworkCore;

namespace IThelpdesk.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UserListDto>> GetAllUsersAsync()
        {
            return await _context.Users

                .OrderBy(u => u.FirstName)

                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate
                })

                .ToListAsync();
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(int id)
        {
            return await _context.Users

                .Where(u => u.UserId == id)

                .Select(u => new UserDetailsDto
                {
                    UserId = u.UserId,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Email = u.Email,
                    Role = u.Role,
                    IsActive = u.IsActive,
                    CreatedDate = u.CreatedDate,
                    LastLoginDate = u.LastLoginDate
                })

                .FirstOrDefaultAsync();
        }

        public async Task<User?> GetUserEntityByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            return Task.CompletedTask;
        }

        public Task DeleteUserAsync(User user)
        {
            _context.Users.Remove(user);
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<User>> GetTechniciansAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Technician")
                .OrderBy(u => u.FirstName)
                .ToListAsync();
        }
    }
}