using MediatR;

namespace PlataformaBancaria.Application.Queries.Contas;

public record ObterContaPorCnpjQuery(string Cnpj) : IRequest<ContaResponseDto?>;