using MediatR;
using PlataformaBancaria.Domain.Repositories;

namespace PlataformaBancaria.Application.Queries.Contas;

public class ObterContaPorIdQueryHandler : IRequestHandler<ObterContaPorIdQuery, ContaResponseDto?>
{
    private readonly IContaRepository _contaRepository;

    public ObterContaPorIdQueryHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<ContaResponseDto?> Handle(ObterContaPorIdQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorIdAsync(request.Id);

        if (conta == null) return null;

        return new ContaResponseDto(conta.Id, conta.Cnpj, conta.RazaoSocial, conta.Agencia, conta.Status);
    }
}