using Microsoft.AspNetCore.Mvc;
using PlataformaBancaria.Application.DTOs;
using PlataformaBancaria.Application.Services.Interfaces;

namespace PlataformaBancaria.Api.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados à Conta.
    /// Atua apenas como camada de apresentação, delegando toda a regra de
    /// negócio para o serviço de aplicação (IContaAppService).
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IContaAppService _contaAppService;

        public ContasController(IContaAppService contaAppService)
        {
            _contaAppService = contaAppService;
        }

        /// <summary>
        /// Cria uma nova Conta a partir dos dados informados no corpo da requisição.
        /// </summary>
        /// <param name="request">Dados necessários para a criação da conta (Cnpj, RazaoSocial, Agencia).</param>
        /// <response code="201">Conta criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou CNPJ já cadastrado.</response>
        [HttpPost]
        [ProducesResponseType(typeof(ContaResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CriarConta([FromBody] CriarContaRequestDto request)
        {
            try
            {
                var conta = await _contaAppService.CriarContaAsync(request);

                // Retorna 201 Created, apontando para o próprio recurso criado.
                // Como ainda não existe um endpoint de consulta por Id (GET),
                // usamos nameof(CriarConta) apenas como referência simbólica da ação.
                return CreatedAtAction(nameof(CriarConta), new { id = conta.Id }, conta);
            }
            catch (ArgumentException ex)
            {
                // Lançada pelo Value Object Cnpj quando o CNPJ é inválido
                // (formato incorreto ou dígitos verificadores não conferem).
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                // Lançada pelo ContaAppService quando o CNPJ já está cadastrado
                // ou pela entidade Conta quando uma regra de negócio é violada.
                return BadRequest(new { erro = ex.Message });
            }
        }
        /// <summary>
        /// Realiza um depósito na conta informada.
        /// </summary>
        /// <param name="request">Dados necessários para o depósito (ContaId e Valor).</param>
        /// <response code="200">Depósito realizado com sucesso.</response>
        /// <response code="400">Valor inválido ou conta não encontrada.</response>
        [HttpPost("deposito")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RealizarDeposito([FromBody] RealizarDepositoRequestDto request)
        {
            try
            {
            await _contaAppService.RealizarDepositoAsync(request);

            return Ok(new { mensagem = "Depósito realizado com sucesso." });
            }
            catch (ArgumentException ex)
            {
            // Lançada pela entidade Conta quando o valor do depósito é inválido (<= 0).
            return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
            // Lançada pelo ContaAppService quando a conta não é encontrada,
            // ou pela entidade Conta quando a conta está bloqueada/encerrada.
        return BadRequest(new { erro = ex.Message });
            }
        }    
    }
}