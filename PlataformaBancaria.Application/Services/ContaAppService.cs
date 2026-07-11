using PlataformaBancaria.Application.DTOs;
using PlataformaBancaria.Application.Services.Interfaces;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Domain.ValueObjects;

namespace PlataformaBancaria.Application.Services
{
    public class ContaAppService : IContaAppService
    {
        private readonly IContaRepository _repository;

        public ContaAppService(IContaRepository repository)
        {
            _repository = repository;
        }

        public async Task<ContaResponseDto> CriarContaAsync(CriarContaRequestDto request)
        {
            var cnpj = new Cnpj(request.Cnpj);

            var contaExistente = await _repository.ObterPorCnpjAsync(cnpj);
            if (contaExistente is not null)
                throw new InvalidOperationException("Já existe uma conta cadastrada com este CNPJ.");

            var conta = new Conta(cnpj, request.RazaoSocial, request.Agencia);

            await _repository.AdicionarAsync(conta);

            return MapearParaResponseDto(conta);
        }

        public async Task RealizarDepositoAsync(RealizarDepositoRequestDto request)
        {
            // 1. Busca a conta pelo Id informado.
            var conta = await _repository.ObterPorIdAsync(request.ContaId);

            // 2. Verifica se a conta existe.
            if (conta is null)
                throw new InvalidOperationException("Conta não encontrada.");

            // 3. Aplica a regra de negócio de depósito, delegada à própria entidade.
            conta.Depositar(request.Valor);

            // 4. Persiste a alteração de saldo através do repositório.
            await _repository.AtualizarAsync(conta);
        }

        private static ContaResponseDto MapearParaResponseDto(Conta conta)
        {
            return new ContaResponseDto(
                conta.Id,
                conta.Cnpj, 
                conta.RazaoSocial,
                conta.Agencia,
                conta.Saldo,
                conta.Status.ToString());
        }
    }
}