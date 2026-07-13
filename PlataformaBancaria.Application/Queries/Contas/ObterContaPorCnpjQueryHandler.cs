using MediatR;
using PlataformaBancaria.Domain.Repositories;

namespace PlataformaBancaria.Application.Queries.Contas;

public class ObterContaPorCnpjQueryHandler : IRequestHandler<ObterContaPorCnpjQuery, ContaResponseDto?>
{
    private readonly IContaRepository _contaRepository;

    public ObterContaPorCnpjQueryHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<ContaResponseDto?> Handle(ObterContaPorCnpjQuery request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorCnpjAsync(request.Cnpj);

        if (conta == null) return null;

        return new ContaResponseDto(conta.Id, conta.Cnpj, conta.RazaoSocial, conta.Agencia, conta.Status);
    }
}