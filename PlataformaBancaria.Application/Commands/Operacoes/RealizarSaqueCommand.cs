using MediatR;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    /// <summary>
    /// Comando para realizar um saque em uma conta.
    /// </summary>
    public record RealizarSaqueCommand(Guid ContaId, string IdempotencyKey, decimal Valor, string Moeda, string Descricao) : IRequest;
}