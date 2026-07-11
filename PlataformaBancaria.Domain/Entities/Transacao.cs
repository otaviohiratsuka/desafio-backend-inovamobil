using System;
using PlataformaBancaria.Domain.Enums;

namespace PlataformaBancaria.Domain.Entities
{
    /// <summary>
    /// Representa uma movimentação (depósito ou saque) ocorrida em uma Conta.
    /// </summary>
    public sealed class Transacao
    {
        public Guid Id { get; private set; }
        public Guid ContaId { get; private set; }
        public TipoTransacao Tipo { get; private set; }
        public decimal Valor { get; private set; }
        public DateTime DataHora { get; private set; }

        // Construtor sem parâmetros exigido internamente pelo Entity Framework Core
        protected Transacao() { }

        public Transacao(Guid contaId, TipoTransacao tipo, decimal valor)
        {
            // Omitimos o Guid.NewGuid() aqui. O Id permanecerá Guid.Empty.
            // O EF Core reconhecerá como uma entidade nova (Added), fará o INSERT
            // e gerará o Guid automaticamente de forma segura.
            ContaId = contaId;
            Tipo = tipo;
            Valor = valor;
            DataHora = DateTime.UtcNow;
        }
    }
}