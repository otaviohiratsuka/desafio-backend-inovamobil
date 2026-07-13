namespace PlataformaBancaria.Application.Events;

public record TransferenciaRealizadaEvent
{
    public Guid ContaOrigemId { get; init; }
    public Guid ContaDestinoId { get; init; }
    public decimal Valor { get; init; }
    public DateTime DataOcorrencia { get; init; }
}