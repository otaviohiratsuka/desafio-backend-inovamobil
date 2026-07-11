using MediatR;
using PlataformaBancaria.Domain.Repositories;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    /// <summary>
    /// Handler responsável por processar o saque em uma conta.
    /// </summary>
    public class RealizarSaqueCommandHandler : IRequestHandler<RealizarSaqueCommand>
    {
        private readonly IContaRepository _repository;

        public RealizarSaqueCommandHandler(IContaRepository repository)
        {
            _repository = repository;
        }

        public async Task Handle(RealizarSaqueCommand request, CancellationToken cancellationToken)
        {
            var conta = await _repository.ObterPorIdAsync(request.ContaId);

            if (conta is null)
                throw new KeyNotFoundException("Conta não encontrada.");

            conta.Sacar(request.Valor);

            await _repository.AtualizarAsync(conta);
        }
    }
}