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
    // Handler responsável por processar o saque em uma conta.
    public class RealizarSaqueCommandHandler : IRequestHandler<RealizarSaqueCommand>
    {
        private readonly IContaRepository _repository;
        private readonly IPublishEndpoint _publishEndpoint;
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

            // Salva no banco de dados principal
            await _repository.AtualizarAsync(conta);

            var evento = new SaqueRealizadoEvent
            {
                ContaId = request.ContaId,
                Valor = request.Valor,
                DataOcorrencia = DateTime.UtcNow
            };

            await _publishEndpoint.Publish(evento, cancellationToken);
        }
    }
}