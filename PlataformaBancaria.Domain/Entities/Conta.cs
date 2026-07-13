using System;
using System.Collections.Generic;
using PlataformaBancaria.Domain.ValueObjects;
using PlataformaBancaria.Domain.Enums;

namespace PlataformaBancaria.Domain.Entities
{
    public class Conta
    {
        public Guid Id { get; private set; }
        public Cnpj Cnpj { get; private set; }
        public string RazaoSocial { get; private set; }
        public string Agencia { get; private set; }
        public decimal Saldo { get; private set; }
        public string Status { get; private set; }

        private readonly List<Transacao> _transacoes = new();
        
        public IReadOnlyCollection<Transacao> Transacoes => _transacoes;

        protected Conta() 
        { 
            Cnpj = null!;
            RazaoSocial = null!;
            Agencia = null!;
            Status = null!;
        }

        public Conta(Cnpj cnpj, string razaoSocial, string agencia)
        {
            Id = Guid.NewGuid();
            Cnpj = cnpj;
            RazaoSocial = razaoSocial;
            Agencia = agencia;
            Saldo = 0m;
            Status = "Ativa";
        }

        public void Depositar(decimal valor)
        {
            GarantirContaOperavel();

            if (valor <= 0)
                throw new ArgumentException("O valor do depósito deve ser maior que zero.");

            Saldo += valor;

            _transacoes.Add(new Transacao(Id, TipoTransacao.Deposito, valor));
        }

        public void Sacar(decimal valor)
        {
            GarantirContaOperavel();

            if (valor <= 0)
                throw new ArgumentException("O valor do saque deve ser maior que zero.");

            if (Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente para realizar esta operação.");

            Saldo -= valor;

            _transacoes.Add(new Transacao(Id, TipoTransacao.Saque, valor));
        }

        private void GarantirContaOperavel()
        {
            if (Status != "Ativa")
                throw new InvalidOperationException("Operação não permitida. A conta não está ativa.");
        }

        public void AlterarStatus(string novoStatus)
        {
        Status = novoStatus;
        }
    }
}