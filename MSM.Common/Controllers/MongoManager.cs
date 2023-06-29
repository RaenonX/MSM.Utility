using MongoDB.Driver;

namespace MSM.Common.Controllers;

public static class MongoManager {
    public static async Task Initialize() {
        MongoConst.Client.Ping();

        await Task.WhenAll(MongoIndexManager.Initialize());
    }

    private static void Ping(this IMongoClient client) {
        try {
            Console.WriteLine($"Testing connection to MongoDB at {MongoConst.Url}");
            client.ListDatabaseNames().MoveNext();
        } catch (TimeoutException) {
            Console.WriteLine($"Error connecting to MongoDB at {MongoConst.Url}");
            Environment.Exit(1);
            throw;
        }
    }
}