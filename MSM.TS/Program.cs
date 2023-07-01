using MSM.Common.Extensions;

var builder = WebApplication.CreateBuilder(args).BuildCommon();

builder.Services.AddCorsFromConfig();
builder.Services.AddControllers();

var app = builder
    .Build()
    .InitLogging();

app.UseCors();
app.MapControllers();

await app.BootAsync();