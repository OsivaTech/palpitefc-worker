using PalpiteFC.Worker.Guesses;
using PalpiteFC.Worker.Repository;
using PalpiteFC.Worker.Integrations.Extensions;
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();
builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Settings:Database:MySql"));
builder.Services.AddDatabase();
builder.Services.AddIntegrationServices(builder.Configuration);
var host = builder.Build();

host.Run();
