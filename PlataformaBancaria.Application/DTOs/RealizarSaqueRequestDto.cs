namespace PlataformaBancaria.Application.DTOs
{
    /// <summary>
    /// DTO utilizado para receber os dados necessários à realização de um saque em conta.
    /// </summary>
    public record RealizarSaqueRequestDto(Guid ContaId, decimal Valor);
}