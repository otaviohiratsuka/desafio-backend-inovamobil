using MediatR;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    public record RealizarDepositoCommand(Guid ContaId, string IdempotencyKey, decimal Valor, string Moeda, string Descricao) : IRequest;
}