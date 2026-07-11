using MediatR;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    /// <summary>
    /// Comando para realizar um depósito em uma conta.
    /// </summary>
    public record RealizarDepositoCommand(Guid ContaId, string IdempotencyKey, decimal Valor, string Moeda, string Descricao) : IRequest;
}