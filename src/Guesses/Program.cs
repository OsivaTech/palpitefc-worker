using PalpiteFC.Worker.Guesses;
using PalpiteFC.Worker.Repository;
using PalpiteFC.Worker.Integrations.Extensions;
using PalpiteFC.Worker.Guesses.Settings;
using System.Drawing;
using PalpiteFC.Worker.Guesses.Util;
using PalpiteFC.Worker.Guesses.Interface;
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Settings:Database:MySql"));
builder.Services.Configure<PointsSettings>(builder.Configuration.GetSection("Settings:PointsConfig:Points"));
builder.Services.AddTransient<IPointsService, PointsService>();
builder.Services.AddDatabase();
builder.Services.AddIntegrationServices(builder.Configuration);
var host = builder.Build();

host.Run();
