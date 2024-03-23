using PalpiteFC.Worker.Games;
using PalpiteFC.Worker.Integrations.Extensions;
using PalpiteFC.Worker.Repository;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Settings:Database:MySql"));

builder.Services.AddIntegrationServices(builder.Configuration);
builder.Services.AddDatabase();

var host = builder.Build();
host.Run();
