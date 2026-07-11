using System;
using PlataformaBancaria.Domain.ValueObjects;

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
            if (valor <= 0)
                throw new ArgumentException("O valor do depósito deve ser maior que zero.");

            Saldo += valor;
        }

        public void Sacar(decimal valor)
        {
            if (valor <= 0)
                throw new ArgumentException("O valor do saque deve ser maior que zero.");

            if (Saldo < valor)
                throw new InvalidOperationException("Saldo insuficiente para realizar esta operação.");

            Saldo -= valor;
        }
    }
}