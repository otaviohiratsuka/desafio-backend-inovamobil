using Microsoft.EntityFrameworkCore;
using MassTransit;
using MongoDB.Driver;
using PlataformaBancaria.Application.Commands.Contas;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Infrastructure.Data;
using PlataformaBancaria.Infrastructure.Repositories;
using PlataformaBancaria.Domain.Services;
using PlataformaBancaria.Infrastructure.Services;

#pragma warning disable CS0618
MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(new MongoDB.Bson.Serialization.Serializers.GuidSerializer(MongoDB.Bson.GuidRepresentation.Standard));
#pragma warning restore CS0618

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddSingleton<IMongoClient>(new MongoClient("mongodb://mongodb:27017"));

builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();
builder.Services.AddHttpClient<IEmpresaService, EmpresaService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq", 5672, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});

builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(typeof(AbrirContaCommand).Assembly));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Aplica as migrações do banco de dados automaticamente ao iniciar
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

app.Run();