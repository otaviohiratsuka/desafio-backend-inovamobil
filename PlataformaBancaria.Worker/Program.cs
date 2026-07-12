using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Worker.Consumers;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((hostContext, services) =>
{
    services.AddSingleton<IMongoClient>(new MongoClient("mongodb://localhost:27018"));

    services.AddMassTransit(x =>
    {
        x.AddConsumer<DepositoRealizadoConsumer>();

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("localhost", 5673, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ReceiveEndpoint("deposito-realizado-queue", e =>
            {
                e.ConfigureConsumer<DepositoRealizadoConsumer>(context);
            });
        });
    });
});

var host = builder.Build();
await host.RunAsync();