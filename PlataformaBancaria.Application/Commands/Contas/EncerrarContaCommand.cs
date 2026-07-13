using MediatR;

namespace PlataformaBancaria.Application.Commands.Contas;

public record EncerrarContaCommand(Guid Id) : IRequest<string>; 
// Retorna uma string com o resultado (ex: "Sucesso", "NaoEncontrada", "SaldoPositivo")