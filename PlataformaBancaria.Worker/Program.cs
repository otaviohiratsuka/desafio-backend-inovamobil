using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Worker.Consumers;

var builder = Host.CreateDefaultBuilder(args);

#pragma warning disable CS0618
MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
#pragma warning restore CS0618

builder.ConfigureServices((hostContext, services) =>
{
    services.AddSingleton<IMongoClient>(new MongoClient("mongodb://mongodb:27017"));

    services.AddMassTransit(x =>
    {
        x.AddConsumer<DepositoRealizadoConsumer>();
        x.AddConsumer<SaqueRealizadoConsumer>();
        x.AddConsumer<TransferenciaRealizadaConsumer>(); // <-- Consumidor de Transferência adicionado

        x.UsingRabbitMq((context, cfg) =>
        {
            cfg.Host("rabbitmq", 5672, "/", h =>
            {
                h.Username("guest");
                h.Password("guest");
            });

            cfg.ReceiveEndpoint("deposito-realizado-queue", e =>
            {
                e.ConfigureConsumer<DepositoRealizadoConsumer>(context);
            });

            cfg.ReceiveEndpoint("saque-realizado-queue", e =>
            {
                e.ConfigureConsumer<SaqueRealizadoConsumer>(context);
            });

            cfg.ReceiveEndpoint("transferencia-realizada-queue", e => 
            {
                e.ConfigureConsumer<TransferenciaRealizadaConsumer>(context);
            });
        });
    });
});

var host = builder.Build();
await host.RunAsync();