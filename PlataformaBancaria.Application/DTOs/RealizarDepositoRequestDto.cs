namespace PlataformaBancaria.Application.DTOs
{
    /// <summary>
    /// DTO utilizado para receber os dados necessários à realização de um depósito em conta.
    /// </summary>
    public record RealizarDepositoRequestDto(Guid ContaId, decimal Valor);
}