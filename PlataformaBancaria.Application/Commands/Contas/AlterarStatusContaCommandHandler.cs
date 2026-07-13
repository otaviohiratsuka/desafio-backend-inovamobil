using MediatR;
using PlataformaBancaria.Domain.Repositories;

namespace PlataformaBancaria.Application.Commands.Contas;

public class AlterarStatusContaCommandHandler : IRequestHandler<AlterarStatusContaCommand, bool>
{
    private readonly IContaRepository _contaRepository;

    public AlterarStatusContaCommandHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<bool> Handle(AlterarStatusContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorIdAsync(request.Id);
        if (conta == null) return false;

        // Regra de negócio: Você pode expor um método na sua Entidade/Aggregate 'Conta' para alterar o status
        conta.AlterarStatus(request.Status); 

        await _contaRepository.AtualizarAsync(conta);
        return true;
    }
}