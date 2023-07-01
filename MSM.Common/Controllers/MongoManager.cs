﻿using MongoDB.Driver;
using MSM.Common.Utils;

namespace MSM.Common.Controllers;

public static class MongoManager {
    private static readonly ILogger Logger = LogHelper.CreateLogger(typeof(MongoManager));

    public static async Task Initialize() {
        MongoConst.Client.Ping();

        await Task.WhenAll(MongoIndexManager.Initialize());
    }

    private static void Ping(this IMongoClient client) {
        try {
            Logger.LogInformation("Testing connection to MongoDB at {MongoUrl}", MongoConst.Url);
            client.ListDatabaseNames().MoveNext();
        } catch (TimeoutException e) {
            Logger.LogError(e, "Error connecting to MongoDB at {MongoUrl}", MongoConst.Url);
            Environment.Exit(1);
            throw;
        }
    }
}