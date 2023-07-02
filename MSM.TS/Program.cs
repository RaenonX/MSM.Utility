using MSM.Common.Extensions;

var builder = WebApplication.CreateBuilder(args).BuildCommon();

builder.Services.AddCorsFromConfig();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();

var app = builder
    .Build()
    .InitLogging();

app.UseCors();
app.MapControllers();
app.MapHealthChecks("/health");

await app.BootAsync();