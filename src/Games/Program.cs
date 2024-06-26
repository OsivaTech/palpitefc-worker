using PalpiteFC.Libraries.Persistence.Database.Connection;
using PalpiteFC.Libraries.Persistence.Database.Extensions;
using PalpiteFC.Libraries.Persistence.Database.Settings;
using PalpiteFC.Worker.Games;
using PalpiteFC.Worker.Games.Interfaces;
using PalpiteFC.Worker.Games.Services;
using PalpiteFC.Worker.Games.Settings;
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

    builder.Services.AddTransient<IFixturesService, FixturesService>();

    builder.Services.AddIntegrationServices(builder.Configuration);
    builder.Services.AddDatabase(true);

    var host = builder.Build();

    using var scope = host.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<DataContext>();
    await context.Init();

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