using Sociala.Models;
using Microsoft.EntityFrameworkCore;

namespace Sociala.Data
{
    public class AppData : DbContext
    {
        public AppData(DbContextOptions<AppData> options)
            : base(options)
        {
        }
        public DbSet<User> User { get; set; }

    }
}
