using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Worker.Consumers;

var builder = Host.CreateDefaultBuilder(args);

#pragma warning disable CS0618
MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
#pragma warning restore CS0618


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