using Microsoft.Extensions.Configuration;
using MongoDB.Driver;

namespace OKeeffeCraft.Database
{
    public class MongoDataContext
    {
        private readonly IConfiguration _configuration;
        public readonly IMongoDatabase db;

        public MongoDataContext(IConfiguration configuration)
        {
            _configuration = configuration;
            MongoClient client = new(_configuration["MongoDBConnectionString"]);
            CheckAndCreateDatabase(client);
            db = client.GetDatabase(_configuration["MongoDBName"]);
        }

        private async void CheckAndCreateDatabase(MongoClient client)
        {
            var databaseName = _configuration["MongoDBName"]; // Replace with your desired database name

            var databaseNames = await client.ListDatabaseNamesAsync();

            
            Console.WriteLine(databaseNames);
            if (!databaseNames.ToList().Contains(databaseName))
            {
                var database = client.GetDatabase(databaseName);
                // Perform any initial setup or configuration for the database

                Console.WriteLine($"Created database: {databaseName}");
            }
            else
            {
                Console.WriteLine($"Database already exists: {databaseName}");
            }
            CheckAndCreateCollections(client);
        }

        private async void CheckAndCreateCollections(MongoClient client)
        {
            var databaseName = _configuration["MongoDBName"]; // Replace with your desired database name

            var database = client.GetDatabase(databaseName);
            if (database == null)
            {
                Console.WriteLine($"Database not found: {databaseName}");
                return;
            }

            var collectionNames = await database.ListCollectionNamesAsync();
            var collectionNamesList = collectionNames.ToList();

            // Check and create collections as needed
            if (!collectionNamesList.Contains("Accounts"))
            {
                database.CreateCollection("Accounts");
            }

            if (!collectionNamesList.Contains("Email"))
            {
                database.CreateCollection("Email");
            }

            if (!collectionNamesList.Contains("ActivityLogs"))
            {
                database.CreateCollection("ActivityLogs");
            }
            if (!collectionNamesList.Contains("ErrorLogs"))
            {
                database.CreateCollection("ErrorLogs");
            }
            if (!collectionNamesList.Contains("RefreshTokens"))
            {
                database.CreateCollection("RefreshTokens");
            }

            // Add more checks and collection creations as needed
        }
    }
}
