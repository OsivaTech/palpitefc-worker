using PalpiteFC.Libraries.Persistence.Database.Extensions;
using PalpiteFC.Libraries.Persistence.Database.Settings;
using PalpiteFC.Worker.Guesses;
using PalpiteFC.Worker.Guesses.Interfaces;
using PalpiteFC.Worker.Guesses.Services;
using PalpiteFC.Worker.Guesses.Settings;
using PalpiteFC.Worker.Integrations.Extensions;
using Serilog;

try
{
    Log.Logger = new LoggerConfiguration()
    .WriteTo.Console(outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] [{ThreadId}] {Message}{NewLine}{Exception}")
    .CreateLogger();

    var builder = Host.CreateApplicationBuilder(args);

    Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

    Log.Information("Configuring service.");

    builder.Services.AddSerilog(Log.Logger);

    builder.Services.AddHostedService<Worker>();

    builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Settings:Database:MySql"));
    builder.Services.Configure<WorkerSettings>(builder.Configuration.GetSection("Settings:Worker"));
    
    builder.Services.AddTransient<IPointsService, PointsService>();
    builder.Services.AddTransient<IGuessesService, GuessesService>();
    
    builder.Services.AddDatabase(true);
    builder.Services.AddIntegrationServices(builder.Configuration);

    var host = builder.Build();

    Log.Information("Service configured. Starting...");

    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly: {Message}", ex.Message);
}
finally
{
    Log.Information("Server Shutting down...");
    Log.CloseAndFlush();
}