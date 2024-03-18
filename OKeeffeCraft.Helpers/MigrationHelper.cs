using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OKeeffeCraft.Database;
using System.Data.Common;

namespace OKeeffeCraft.Helpers
{
    public static class MigrationHelper
    {
        public static void RunMigrations<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetRequiredService<DataContext>();

                Console.WriteLine("Checking for database existence...");

                if (!dataContext.Database.CanConnect())
                {
                    Console.WriteLine("Database does not exist. Creating the database...");
                    dataContext.Database.EnsureCreated();
                    Console.WriteLine("Database created successfully.");
                }
                else
                {
                    Console.WriteLine("Database already exists.");
                }

                Console.WriteLine("Checking for migration table existence...");

                if (!MigrationTableExists(dataContext))
                {
                    Console.WriteLine("Migration table does not exist. Applying initial migrations...");
                    dataContext.Database.Migrate();
                    Console.WriteLine("Initial migrations applied successfully.");
                }
                else
                {
                    Console.WriteLine("Migration table already exists. No migrations needed.");
                }
            }
        }

        private static bool MigrationTableExists(DbContext dbContext)
        {
            try
            {
                // Check if the __EFMigrationsHistory table exists in the database
                return dbContext.Database.ExecuteSqlRaw("SELECT 1 FROM __EFMigrationsHistory LIMIT 1") != -1;
            }
            catch (DbException)
            {
                // Table does not exist
                return false;
            }
        }
    }
}

