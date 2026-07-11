using PlataformaBancaria.Application.Services.Interfaces;
using PlataformaBancaria.Application.Services;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using PlataformaBancaria.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Registra o DbContext e aponta para a connection string
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// Registra os repositórios e serviços (Injeção de Dependência)
builder.Services.AddScoped<IContaRepository, ContaRepository>();
builder.Services.AddScoped<IContaAppService, ContaAppService>();

// Adiciona o suporte para as rotas da API e documentação Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configura o pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Mapeia os Controllers e inicia a API
app.MapControllers();

app.Run();