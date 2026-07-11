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
    }
}