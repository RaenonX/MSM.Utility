using MongoDB.Driver;
using MSM.Common.Extensions;
using MSM.Common.Models;
using MSM.Common.Utils;

namespace MSM.Common.Controllers;


public static class MongoConst {
    public static readonly string Url = ConfigHelper.GetMongoDbUrl();
    
    public static readonly IMongoClient Client = new MongoClient(Url).Initialize();

    public static readonly IMongoDatabase PxTickDatabase = Client.GetDatabase("pxTick");

    private static readonly IMongoDatabase PxCalcDatabase = Client.GetDatabase("pxCalc");

    // One of the usages for meta database is to use Change Stream to trigger price watch event
    private static readonly IMongoDatabase PxMetaDatabase = Client.GetDatabase("pxMeta");

    private static readonly IMongoDatabase AlertDatabase = Client.GetDatabase("alert");

    public static readonly IMongoCollection<PxMetaModel> PxMetaCollection =
        PxMetaDatabase.GetCollection<PxMetaModel>("meta");

    public static readonly IMongoCollection<PxAlertModel> PxAlertCollection =
        AlertDatabase.GetCollection<PxAlertModel>("px");

    public static IMongoCollection<PxDataModel> GetPxTickCollection(string itemName) {
        return PxTickDatabase.GetCollection<PxDataModel>(itemName);
    }

    public static IMongoCollection<PxDataModel> GetPxCalcCollection(string itemName) {
        return PxCalcDatabase.GetCollection<PxDataModel>(itemName);
    }
}