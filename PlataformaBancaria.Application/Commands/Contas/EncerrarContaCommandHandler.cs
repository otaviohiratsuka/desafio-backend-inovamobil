using MediatR;
using PlataformaBancaria.Domain.Repositories;

namespace PlataformaBancaria.Application.Commands.Contas;

public class EncerrarContaCommandHandler : IRequestHandler<EncerrarContaCommand, string>
{
    private readonly IContaRepository _contaRepository;

    public EncerrarContaCommandHandler(IContaRepository contaRepository)
    {
        _contaRepository = contaRepository;
    }

    public async Task<string> Handle(EncerrarContaCommand request, CancellationToken cancellationToken)
    {
        var conta = await _contaRepository.ObterPorIdAsync(request.Id);
        if (conta == null) return "NaoEncontrada";

        // Critério Eliminatório / Regra de Negócio: Só encerra com saldo ZERO
        if (conta.Saldo > 0) 
        {
            return "SaldoPositivo";
        }

        // Executa o soft delete alterando para "Encerrada"
        conta.AlterarStatus("Encerrada"); 

        await _contaRepository.AtualizarAsync(conta);
        return "Sucesso";
    }
}