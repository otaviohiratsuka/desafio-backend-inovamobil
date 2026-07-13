using MediatR;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    public record RealizarSaqueCommand(Guid ContaId, string IdempotencyKey, decimal Valor, string Moeda, string Descricao) : IRequest;
}