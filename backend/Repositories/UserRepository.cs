using IThelpdesk.Data;
using IThelpdesk.DTOs.Common;
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

        public async Task<PagedResultDto<UserListDto>> GetAllUsersAsync(
     int pageNumber,
     int pageSize,
     string? search)
        {
            var query = _context.Users
                .AsNoTracking()
                .AsQueryable();

            //--------------------------------------------------
            // Search
            //--------------------------------------------------

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                query = query.Where(u =>
                    u.FirstName.Contains(search) ||
                    u.LastName.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Role.Contains(search));
            }

            //--------------------------------------------------
            // Total Count
            //--------------------------------------------------

            var totalCount = await query.CountAsync();

            //--------------------------------------------------
            // Pagination
            //--------------------------------------------------

            var items = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .ThenBy(u => u.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            //--------------------------------------------------
            // Return Paged Result
            //--------------------------------------------------

            return new PagedResultDto<UserListDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
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

        public async Task<IEnumerable<User>> GetAdminsAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Admin" && u.IsActive)
                .ToListAsync();
        }
    }
}