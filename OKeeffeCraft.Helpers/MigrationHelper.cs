using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using OKeeffeCraft.Database;
using System.Data;
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
                    Console.WriteLine("Migration table already exists. Checking for missing tables...");

                    // Check for missing tables and apply migrations if necessary
                    Console.WriteLine("Checking for missing tables...");

                    // Get the list of all entity types
                    var entityTypes = dataContext.Model.GetEntityTypes().Select(e => e.GetTableName()).ToList();

                    // Get the list of tables existing in the database
                    var existingTables = dataContext.Database.GetTables();

                    // Check for missing tables
                    var missingTables = entityTypes.Except(existingTables).ToList();

                    if (missingTables.Any())
                    {
                        Console.WriteLine($"Missing tables found: {string.Join(", ", missingTables)}");

                        // Apply migrations
                        Console.WriteLine("Applying migrations to create missing tables...");
                        // 4. If there are missing tables, create and apply a new migration
                        CreateAndApplyMigration<TContext>(serviceProvider);
                        Console.WriteLine("Migrations applied successfully.");
                    }
                    else
                    {
                        Console.WriteLine("No missing tables found. Database is up-to-date.");
                    }
                }
            }
        }
        private static List<string> GetTables(this DatabaseFacade database)
        {
            var tables = new List<string>();

            var connection = database.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
                connection.Open();

            var commandText = "SELECT name FROM sqlite_master WHERE type = 'table'";

            var command = connection.CreateCommand();
            command.CommandText = commandText;

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }

            return tables;
        }

        private static bool MigrationTableExists(DbContext dbContext)
        {
            try
            {
                // Check if the __EFMigrationsHistory table exists in the database
                return dbContext.Database.ExecuteSqlRaw("SELECT 1 FROM __EFMigrationsHistory LIMIT 1") != 0;
            }
            catch (DbException)
            {
                // Table does not exist
                return false;
            }
        }

        private static void CreateAndApplyMigration<TContext>(IServiceProvider serviceProvider) where TContext : DbContext
        {
            var dbContext = serviceProvider.GetRequiredService<TContext>();
            var database = dbContext.Database;

            // Apply migrations
            database.Migrate();
        }
    }
}

