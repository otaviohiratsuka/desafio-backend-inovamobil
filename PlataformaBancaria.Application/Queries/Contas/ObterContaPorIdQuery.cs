using MediatR;

namespace PlataformaBancaria.Application.Queries.Contas;

public record ObterContaPorIdQuery(Guid Id) : IRequest<ContaResponseDto?>;

public record ContaResponseDto(Guid Id, string Cnpj, string RazaoSocial, string Agencia, string Status);