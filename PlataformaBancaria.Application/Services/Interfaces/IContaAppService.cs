using PlataformaBancaria.Application.DTOs;

namespace PlataformaBancaria.Application.Services.Interfaces
{
    /// <summary>
    /// Contrato do serviço de aplicação (Use Cases) responsável por orquestrar
    /// as operações relacionadas à Conta.
    /// </summary>
    public interface IContaAppService
    {
        /// <summary>
        /// Cria uma nova Conta a partir dos dados informados.
        /// </summary>
        Task<ContaResponseDto> CriarContaAsync(CriarContaRequestDto request);

        /// <summary>
        /// Realiza um depósito na conta informada.
        /// </summary>
        Task RealizarDepositoAsync(RealizarDepositoRequestDto request);

        /// <summary>
        /// Realiza um saque na conta informada, aplicando as regras de negócio
        /// da entidade Conta (valor positivo e saldo suficiente) e persistindo
        /// a alteração através do repositório.
        /// </summary>
        Task RealizarSaqueAsync(RealizarSaqueRequestDto request);

        /// <summary>
        /// Obtém os dados de uma conta a partir do seu Id.
        /// Lança KeyNotFoundException caso a conta não seja encontrada.
        /// </summary>
        Task<ContaResponseDto> ObterContaPorIdAsync(Guid id);

        /// <summary>
        /// Obtém todas as contas cadastradas, já mapeadas para o DTO de resposta.
        /// </summary>
        Task<IEnumerable<ContaResponseDto>> ObterTodasContasAsync();

    }
}