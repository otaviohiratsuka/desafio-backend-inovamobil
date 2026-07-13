using MediatR;

namespace PlataformaBancaria.Application.Queries.Contas;

public record ObterSaldoQuery(Guid ContaId) : IRequest<decimal>;