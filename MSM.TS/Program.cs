using MSM.Common.Controllers;
using MSM.Common.Utils;

var builder = WebApplication.CreateBuilder(args);

ConfigHelper.Initialize(builder.Configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

await MongoManager.Initialize();

app.Run();