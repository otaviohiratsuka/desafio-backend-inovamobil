namespace PlataformaBancaria.Application.DTOs
{
    public record RealizarSaqueRequestDto(Guid ContaId, decimal Valor);
}