using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OKeeffeCraft.Entities;

namespace OKeeffeCraft.Database
{
    public class DataContext : DbContext
    {
        public DbSet<Account> Accounts { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }

        private readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sqlite database
            options.UseSqlite(Configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
