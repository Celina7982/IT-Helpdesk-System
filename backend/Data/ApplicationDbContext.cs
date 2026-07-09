using Microsoft.EntityFrameworkCore;
using IThelpdesk.Models;

namespace IThelpdesk.Data
{     //class connects code to sql server
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}