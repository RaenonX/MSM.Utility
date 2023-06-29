using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace MSM.Common.Extensions;


public static class MongoExtensions {
    public static IMongoClient Initialize(this IMongoClient client) {
        RegisterConvention();
        RegisterSerializer();

        return client;
    }
    
    public static async Task<IEnumerable<string>> GetCollectionNames(this IMongoDatabase database) {
        var cursor = await database.ListCollectionNamesAsync();
        
        return await cursor.ToListAsync();
    } 

    private static void RegisterConvention() {
        ConventionRegistry.Register(
            name: "CamelCaseConvention",
            conventions: new ConventionPack { new CamelCaseElementNameConvention() },
            filter: _ => true
        );
    }

    private static void RegisterSerializer() {
        RegisterGlobalSerializer();
    }

    private static void RegisterGlobalSerializer() {
        // By default, `decimal` are stored in `string`, which is undesired
        BsonSerializer.RegisterSerializer(new DecimalSerializer(BsonType.Decimal128));
    }
}