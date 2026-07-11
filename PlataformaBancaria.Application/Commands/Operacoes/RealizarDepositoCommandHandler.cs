using MediatR;
using MassTransit;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Application.Events;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    public class RealizarDepositoCommandHandler : IRequestHandler<RealizarDepositoCommand>
    {
        private readonly IContaRepository _repository;
        private readonly IIdempotenciaRepository _idempotenciaRepository;
        private readonly IPublishEndpoint _publishEndpoint;

        public RealizarDepositoCommandHandler(
            IContaRepository repository, 
            IIdempotenciaRepository idempotenciaRepository,
            IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _idempotenciaRepository = idempotenciaRepository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(RealizarDepositoCommand request, CancellationToken cancellationToken)
        {
            if (await _idempotenciaRepository.ExisteAsync(request.IdempotencyKey))
            {
                return; 
            }

            var conta = await _repository.ObterPorIdAsync(request.ContaId);

            if (conta is null)
                throw new KeyNotFoundException("Conta não encontrada.");

            conta.Depositar(request.Valor);

            await _repository.AtualizarAsync(conta);

            await _idempotenciaRepository.AdicionarAsync(request.IdempotencyKey);

            var evento = new DepositoRealizadoEvent
            {
                ContaId = conta.Id,
                Valor = request.Valor,
                DataOcorrencia = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(evento, cancellationToken);
        }
    }
}