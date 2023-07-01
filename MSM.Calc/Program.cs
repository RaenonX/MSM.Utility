using MSM.Calc;
using MSM.Calc.Services;
using MSM.Common.Extensions;


var builder = WebApplication.CreateBuilder(args)
    .InitConfig();

builder.Services.AddHostedService<Worker>();
builder.Services.AddGrpc();

var app = builder
    .Build()
    .InitLogging();

app.MapGrpcService<CalculatorService>();

await app.BootAsync();
