using MediatR;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Domain.ValueObjects;
using PlataformaBancaria.Domain.Services;

namespace PlataformaBancaria.Application.Commands.Contas
{
    /// <summary>
    /// Handler responsável por processar a abertura de uma nova Conta.
    /// </summary>
    public class AbrirContaCommandHandler : IRequestHandler<AbrirContaCommand, Guid>
    {
        private readonly IContaRepository _repository;
        private readonly IEmpresaService _empresaService;

        public AbrirContaCommandHandler(
            IContaRepository repository, 
            IEmpresaService empresaService)
        {
            _repository = repository;
            _empresaService = empresaService;
        }

        public async Task<Guid> Handle(AbrirContaCommand request, CancellationToken cancellationToken)
        {
            var cnpj = new Cnpj(request.Cnpj);

            var contaExistente = await _repository.ObterPorCnpjAsync(cnpj);
            if (contaExistente is not null)
                throw new InvalidOperationException("Já existe uma conta cadastrada com este CNPJ.");

            // Chamada real para a API da ReceitaWS
            var razaoSocial = await _empresaService.ObterRazaoSocialPorCnpjAsync(request.Cnpj);

            var conta = new Conta(cnpj, razaoSocial, request.Agencia);

            await _repository.AdicionarAsync(conta);

            return conta.Id;
        }
    }
}