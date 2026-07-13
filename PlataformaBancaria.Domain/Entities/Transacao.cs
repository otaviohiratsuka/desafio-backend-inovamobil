using System;
using PlataformaBancaria.Domain.Enums;

namespace PlataformaBancaria.Domain.Entities
{
    // movimentação (depósito ou saque)
    public sealed class Transacao
    {
        public Guid Id { get; private set; }
        public Guid ContaId { get; private set; }
        public TipoTransacao Tipo { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataHora { get; private set; }
        protected Transacao() { }

        public Transacao(Guid contaId, TipoTransacao tipo, decimal valor)
        {
            ContaId = contaId;
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.UtcNow;
        }
    }
}