using MassTransit;
using PalpiteFC.Libraries.Persistence.Database.Extensions;
using PalpiteFC.Libraries.Persistence.Database.Settings;
using PalpiteFC.Worker.Persistence.Interfaces;
using PalpiteFC.Worker.Persistence.Services;
using PalpiteFC.Worker.QueueManager.Consumers;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<GuessConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("148.113.183.239", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint(x =>
        {
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
host.Run();