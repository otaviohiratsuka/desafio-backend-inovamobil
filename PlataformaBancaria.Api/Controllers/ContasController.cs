using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlataformaBancaria.Application.Commands.Contas;
using PlataformaBancaria.Application.Commands.Operacoes;

namespace PlataformaBancaria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContasController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ContasController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Abre uma nova Conta a partir do CNPJ, agência e documento informados.
        /// </summary>
        /// <response code="201">Conta criada com sucesso.</response>
        /// <response code="400">Dados inválidos ou CNPJ já cadastrado.</response>
        [HttpPost("/api/v1/accounts")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AbrirConta([FromBody] AbrirContaCommand command)
        {
            try
            {
                var contaId = await _mediator.Send(command);
                return Created($"/api/v1/accounts/{contaId}", new { id = contaId });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Realiza um depósito na conta informada pela rota.
        /// </summary>
        /// <response code="200">Depósito realizado com sucesso.</response>
        /// <response code="400">Valor inválido ou conta inoperante.</response>
        /// <response code="404">Conta não encontrada.</response>
        [HttpPost("/api/v1/accounts/{id}/deposit")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RealizarDeposito([FromRoute] Guid id, [FromBody] RealizarDepositoCommand command)
        {
            try
            {
                await _mediator.Send(command with { ContaId = id });
                return Ok(new { mensagem = "Depósito realizado com sucesso." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

        /// <summary>
        /// Realiza um saque na conta informada pela rota.
        /// </summary>
        /// <response code="200">Saque realizado com sucesso.</response>
        /// <response code="400">Valor inválido ou saldo insuficiente.</response>
        /// <response code="404">Conta não encontrada.</response>
        [HttpPost("/api/v1/accounts/{id}/withdraw")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RealizarSaque([FromRoute] Guid id, [FromBody] RealizarSaqueCommand command)
        {
            try
            {
                await _mediator.Send(command with { ContaId = id });
                return Ok(new { mensagem = "Saque realizado com sucesso." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }
    }
}