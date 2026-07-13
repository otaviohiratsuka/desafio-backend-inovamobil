using MediatR;
using System.Text.Json.Serialization;

namespace PlataformaBancaria.Application.Commands.Operacoes;

public record RealizarTransferenciaCommand(
    string IdempotencyKey,
    Guid ContaDestinoId,
    decimal Valor,
    string Moeda,
    string Descricao) : IRequest
{
    [JsonIgnore]
    public Guid ContaOrigemId { get; set; }
}