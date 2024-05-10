using MassTransit;
using PalpiteFC.Libraries.Persistence.Database.Connection;
using PalpiteFC.Libraries.Persistence.Database.Extensions;
using PalpiteFC.Libraries.Persistence.Database.Settings;
using PalpiteFC.Worker.Persistence.Interfaces;
using PalpiteFC.Worker.Persistence.Services;
using PalpiteFC.Worker.QueueManager.Consumers;
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

    builder.Services.AddMassTransit(x =>
    {
        x.AddConsumer<GuessConsumer>();
        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host(builder.Configuration.GetValue<string>("Settings:RabbitMQ:Host"), "/", h =>
            {
                h.Username(builder.Configuration.GetValue<string>("Settings:RabbitMQ:Username"));
                h.Password(builder.Configuration.GetValue<string>("Settings:RabbitMQ:Password"));
            });

            cfg.ReceiveEndpoint(x =>
            {
                x.Durable = false;
                x.AutoDelete = true;

                x.PrefetchCount = 10;
                x.ConcurrentMessageLimit = 2;
                
                x.ConfigureConsumer<GuessConsumer>(context);
            });
        });
    });
    builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("Settings:Database:MySql"));
    builder.Services.AddDatabase(true);
    builder.Services.AddTransient<IGuessService, GuessService>();

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