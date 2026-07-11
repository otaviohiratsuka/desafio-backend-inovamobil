using MediatR;

namespace PlataformaBancaria.Application.Commands.Contas
{
    /// <summary>
    /// Comando para abertura de uma nova Conta, incluindo a imagem do
    /// documento de identificação em Base64 para posterior validação/armazenamento.
    /// </summary>
    public record AbrirContaCommand(string Cnpj, string Agencia, string ImagemDocumento) : IRequest<Guid>;
}