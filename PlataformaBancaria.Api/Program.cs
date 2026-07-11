using Microsoft.EntityFrameworkCore;
using MassTransit;
using PlataformaBancaria.Application.Commands.Contas;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Infrastructure.Data;
using PlataformaBancaria.Infrastructure.Repositories;
using PlataformaBancaria.Domain.Services;
using PlataformaBancaria.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IIdempotenciaRepository, IdempotenciaRepository>();
builder.Services.AddHttpClient<IEmpresaService, EmpresaService>();

builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", 5673, "/", h => {
            h.Username("guest");
            h.Password("guest");
        });
    });
});
// ----------------------------------------------

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

app.Run();