using MediatR;

namespace PlataformaBancaria.Application.Commands.Contas;

public record AlterarStatusContaCommand(Guid Id, string Status) : IRequest<bool>;