using MassTransit;
using MediatR;
using PlataformaBancaria.Application.Events;
using PlataformaBancaria.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PlataformaBancaria.Application.Commands.Operacoes
{
    /// <summary>
    /// Handler responsável por processar o saque em uma conta.
    /// </summary>
    public class RealizarSaqueCommandHandler : IRequestHandler<RealizarSaqueCommand>
    {
        private readonly IContaRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;

        // 1. Injetamos o IPublishEndpoint do MassTransit no construtor
        public RealizarSaqueCommandHandler(IContaRepository repository, IPublishEndpoint publishEndpoint)
        {
            _repository = repository;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Handle(RealizarSaqueCommand request, CancellationToken cancellationToken)
        {
            var conta = await _repository.ObterPorIdAsync(request.ContaId);

            if (conta is null)
                throw new KeyNotFoundException("Conta não encontrada.");

            conta.Sacar(request.Valor);

            // Salva no banco de dados principal (PostgreSQL)
            await _repository.AtualizarAsync(conta);

            // 2. Cria o evento com os dados do saque
            var evento = new SaqueRealizadoEvent
            {
                ContaId = request.ContaId,
                Valor = request.Valor,
                DataOcorrencia = DateTime.UtcNow
            };

            // 3. Publica a mensagem no RabbitMQ para o Worker ouvir
            await _publishEndpoint.Publish(evento, cancellationToken);
        }
    }
}