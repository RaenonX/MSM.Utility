using MongoDB.Driver;
using MSM.Common.Extensions;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Common.Controllers;


public static class MongoConst {
    public static readonly string Url = ConfigHelper.GetMongoDbUrl();
    
    public static readonly IMongoClient Client = new MongoClient(Url).Initialize();

    public static readonly IMongoDatabase PxDatabase = Client.GetDatabase("px");

    private static readonly IMongoDatabase AlertDatabase = Client.GetDatabase("alert");

    public static readonly IMongoCollection<PxAlertModel> PxAlertCollection =
        AlertDatabase.GetCollection<PxAlertModel>("px");

    public static IMongoCollection<PxDataModel> GetPxCollection(string itemName) {
        return PxDatabase.GetCollection<PxDataModel>(itemName);
    }
}