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
        // 1. Controle de Idempotência
        if (await _idempotenciaRepository.ExisteAsync(request.IdempotencyKey))
        {
            return;
        }

        // 2. Busca e Valida as Contas
        var contaOrigem = await _repository.ObterPorIdAsync(request.ContaOrigemId);
        if (contaOrigem is null)
            throw new KeyNotFoundException("Conta de origem não encontrada.");

        var contaDestino = await _repository.ObterPorIdAsync(request.ContaDestinoId);
        if (contaDestino is null)
            throw new KeyNotFoundException("Conta de destino não encontrada.");

        // 3. Regra de Negócio: Ambas precisam estar "Ativa"
        if (contaOrigem.Status != "Ativa" || contaDestino.Status != "Ativa")
            throw new InvalidOperationException("Ambas as contas precisam estar com o status 'Ativa' para realizar uma transferência.");

        // 4. Regra de Negócio: Saldo não pode ficar negativo
        if (contaOrigem.Saldo < request.Valor)
            throw new InvalidOperationException("Saldo insuficiente para realizar a transferência.");

        // 5. Executa as operações
        // (Certifique-se de que sua entidade Conta tenha o método Sacar implementado)
        contaOrigem.Sacar(request.Valor);
        contaDestino.Depositar(request.Valor);

        // 6. Persiste as mudanças
        await _repository.AtualizarAsync(contaOrigem);
        await _repository.AtualizarAsync(contaDestino);

        // 7. Salva a chave de Idempotência
        await _idempotenciaRepository.AdicionarAsync(request.IdempotencyKey);

        // 8. Publica o evento no RabbitMQ
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