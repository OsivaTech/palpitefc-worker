using PalpiteFC.Worker.Guesses;
using PalpiteFC.Worker.Integrations.Extensions;
using PalpiteFC.Worker.Guesses.Settings;
using PalpiteFC.Worker.Repository.Extensions;
using PalpiteFC.Worker.Repository.Settings;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddHostedService<Worker>();

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Settings:Database:MySql"));
builder.Services.Configure<PointsSettings>(builder.Configuration.GetSection("Settings:PointsConfig:Points"));
builder.Services.AddTransient<IPointsService, PointsService>();
builder.Services.AddDatabase();
builder.Services.AddIntegrationServices(builder.Configuration);

var host = builder.Build();

host.Run();
