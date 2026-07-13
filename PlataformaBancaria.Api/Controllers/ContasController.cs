using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlataformaBancaria.Application.Commands.Contas;
using PlataformaBancaria.Application.Commands.Operacoes;
using PlataformaBancaria.Application.Queries;
using PlataformaBancaria.Application.Queries.Contas;

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
            catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
        }

        /// <summary>
        /// Obtém os dados de uma conta específica pelo ID.
        /// </summary>
        [HttpGet("/api/v1/accounts/{id:guid}")]
        [ProducesResponseType(typeof(ContaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorId([FromRoute] Guid id)
        {
            var query = new ObterContaPorIdQuery(id);
            var conta = await _mediator.Send(query);
            
            if (conta == null) return NotFound(new { erro = "Conta não encontrada." });
            return Ok(conta);
        }

        /// <summary>
        /// Obtém os dados de uma conta específica pelo CNPJ.
        /// </summary>
        [HttpGet("/api/v1/accounts/cnpj/{cnpj}")]
        [ProducesResponseType(typeof(ContaResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterPorCnpj([FromRoute] string cnpj)
        {
            var query = new ObterContaPorCnpjQuery(cnpj);
            var conta = await _mediator.Send(query);

            if (conta == null) return NotFound(new { erro = "Conta não encontrada." });
            return Ok(conta);
        }

        /// <summary>
        /// Consulta o saldo atual da conta no banco de leitura (MongoDB).
        /// </summary>
        [HttpGet("/api/v1/accounts/{id}/balance")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ObterSaldo([FromRoute] Guid id)
        {
            var query = new ObterSaldoQuery(id);
            var saldo = await _mediator.Send(query);
            return Ok(new { saldo = saldo });
        }

        /// <summary>
        /// Altera o status de uma conta existente.
        /// </summary>
        [HttpPatch("/api/v1/accounts/{id:guid}/status")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AlterarStatus([FromRoute] Guid id, [FromBody] AlterarStatusRequest request)
        {
            var command = new AlterarStatusContaCommand(id, request.Status);
            var sucesso = await _mediator.Send(command);

            if (!sucesso) return NotFound(new { erro = "Conta não encontrada para alteração de status." });
            return NoContent();
        }

        /// <summary>
        /// Encerra uma conta (soft delete), desde que o saldo seja zero.
        /// </summary>
        [HttpDelete("/api/v1/accounts/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> EncerrarConta([FromRoute] Guid id)
        {
            var command = new EncerrarContaCommand(id);
            var resultado = await _mediator.Send(command);

            return resultado switch
            {
                "NaoEncontrada" => NotFound(new { erro = "Conta não encontrada." }),
                "SaldoPositivo" => BadRequest(new { erro = "Não é possível encerrar uma conta com saldo superior a zero." }),
                "Sucesso" => NoContent(),
                _ => BadRequest()
            };
        }

        /// <summary>
        /// Realiza um depósito na conta informada pela rota.
        /// </summary>
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
            catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        }

        /// <summary>
        /// Realiza um saque na conta informada pela rota.
        /// </summary>
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
            catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        }

        /// <summary>
        /// Realiza uma transferência entre contas.
        /// </summary>
        [HttpPost("/api/v1/accounts/{id}/transfer")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RealizarTransferencia([FromRoute] Guid id, [FromBody] RealizarTransferenciaCommand command)
        {
            try
            {
                command.ContaOrigemId = id; 
                await _mediator.Send(command);
                return Ok(new { mensagem = "Transferência realizada com sucesso." });
            }
            catch (ArgumentException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (InvalidOperationException ex) { return BadRequest(new { erro = ex.Message }); }
            catch (KeyNotFoundException ex) { return NotFound(new { erro = ex.Message }); }
        }

        /// <summary>
        /// Obtém o extrato da conta informada pela rota diretamente do banco de leitura.
        /// </summary>
        [HttpGet("/api/v1/accounts/{id}/statement")]
        [ProducesResponseType(typeof(IEnumerable<TransacaoDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> ObterExtrato(
            [FromRoute] Guid id,
            [FromQuery] DateTime? dataInicio,
            [FromQuery] DateTime? dataFim,
            [FromQuery] string? tipo,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamanhoPagina = 20)
        {
            var query = new ObterExtratoQuery
            {
                ContaId = id,
                DataInicio = dataInicio,
                DataFim = dataFim,
                Tipo = tipo,
                Pagina = pagina,
                TamanhoPagina = tamanhoPagina
            };

            var extrato = await _mediator.Send(query);
            return Ok(extrato);
        }
    }

    public record AlterarStatusRequest(string Status);
}