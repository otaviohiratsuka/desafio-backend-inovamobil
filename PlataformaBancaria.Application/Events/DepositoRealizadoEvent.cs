using System;

namespace PlataformaBancaria.Application.Events
{
    public class DepositoRealizadoEvent
    {
        public Guid ContaId { get; set; }
        public decimal Valor { get; set; }
        public DateTime DataOcorrencia { get; set; }
    }
}