using MSM.Common.Extensions;

var builder = WebApplication.CreateBuilder(args)
    .InitConfig();

builder.Services.AddControllers();

var app = builder.Build();

app.MapControllers();

await app.BootAsync();