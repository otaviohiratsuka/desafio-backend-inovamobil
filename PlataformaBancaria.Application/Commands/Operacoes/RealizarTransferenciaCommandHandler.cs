using MediatR;
using MassTransit;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Application.Events;

namespace PlataformaBancaria.Application.Commands.Operacoes;

public class RealizarTransferenciaCommandHandler : IRequestHandler<RealizarTransferenciaCommand>
{
    private readonly IContaRepository _repository;
    private readonly IIdempotenciaRepository _idempotenciaRepository;
    private readonly IPublishEndpoint _publishEndpoint;

    public RealizarTransferenciaCommandHandler(
        IContaRepository repository,
        IIdempotenciaRepository idempotenciaRepository,
        IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _idempotenciaRepository = idempotenciaRepository;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(RealizarTransferenciaCommand request, CancellationToken cancellationToken)
    {
        if (await _idempotenciaRepository.ExisteAsync(request.IdempotencyKey))
        {
            return;
        }

        var contaOrigem = await _repository.ObterPorIdAsync(request.ContaOrigemId);
        if (contaOrigem is null)
            throw new KeyNotFoundException("Conta de origem não encontrada.");

        var contaDestino = await _repository.ObterPorIdAsync(request.ContaDestinoId);
        if (contaDestino is null)
            throw new KeyNotFoundException("Conta de destino não encontrada.");

        if (contaOrigem.Status != "Ativa" || contaDestino.Status != "Ativa")
            throw new InvalidOperationException("Ambas as contas precisam estar com o status 'Ativa' para realizar uma transferência.");

        if (contaOrigem.Saldo < request.Valor)
            throw new InvalidOperationException("Saldo insuficiente para realizar a transferência.");

        contaOrigem.Sacar(request.Valor);
        contaDestino.Depositar(request.Valor);

        await _repository.AtualizarAsync(contaOrigem);
        await _repository.AtualizarAsync(contaDestino);

        await _idempotenciaRepository.AdicionarAsync(request.IdempotencyKey);

        var evento = new TransferenciaRealizadaEvent
        {
            ContaOrigemId = contaOrigem.Id,
            ContaDestinoId = contaDestino.Id,
            Valor = request.Valor,
            DataOcorrencia = DateTime.UtcNow
        };

        await _publishEndpoint.Publish(evento, cancellationToken);
    }
}