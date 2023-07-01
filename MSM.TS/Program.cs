using MSM.Common.Extensions;

var builder = WebApplication.CreateBuilder(args).BuildCommon();

builder.Services.AddControllers();

var app = builder
    .Build()
    .InitLogging();

app.MapControllers();

await app.BootAsync();