using MediatR;

namespace PlataformaBancaria.Application.Commands.Contas
{
    public record AbrirContaCommand(string Cnpj, string Agencia, string ImagemDocumento) : IRequest<Guid>;
}