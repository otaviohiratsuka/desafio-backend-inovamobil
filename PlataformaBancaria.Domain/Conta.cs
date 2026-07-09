using PlataformaBancaria.Domain.ValueObjects;

namespace PlataformaBancaria.Domain.Entities
{
    public enum StatusConta
    {
        Ativa,
        Bloqueada,
        Encerrada
    }

    public sealed class Conta
    {
        public Guid Id { get; private set; }
        public Cnpj Cnpj { get; private set; }
        public string RazaoSocial { get; private set; }
        public string Agencia { get; private set; }
        public decimal Saldo { get; private set; }
        public StatusConta Status { get; private set; }

        public Conta(Cnpj cnpj, string razaoSocial, string agencia)
        {
            if (cnpj is null)
                throw new ArgumentException("O CNPJ é obrigatório.", nameof(cnpj));

            if (string.IsNullOrWhiteSpace(razaoSocial))
                throw new ArgumentException("A razão social é obrigatória.", nameof(razaoSocial));

            if (string.IsNullOrWhiteSpace(agencia))
                throw new ArgumentException("A agência é obrigatória.", nameof(agencia));

            Id = Guid.NewGuid();
            Cnpj = cnpj;
            RazaoSocial = razaoSocial;
            Agencia = agencia;
            Saldo = 0m;
            Status = StatusConta.Ativa;
        }

        public void Depositar(decimal valor)
        {
            GarantirContaOperavel();

            if (valor <= 0)
                throw new ArgumentException("O valor do depósito deve ser maior que zero.", nameof(valor));

            Saldo += valor;
        }

        public void Sacar(decimal valor)
        {
            GarantirContaOperavel();

            if (valor <= 0)
                throw new ArgumentException("O valor do saque deve ser maior que zero.", nameof(valor));

            if (valor > Saldo)
                throw new InvalidOperationException("Saldo insuficiente para realizar o saque.");

            Saldo -= valor;
        }

        public void Bloquear()
        {
            if (Status == StatusConta.Encerrada)
                throw new InvalidOperationException("Não é possível bloquear uma conta encerrada.");

            Status = StatusConta.Bloqueada;
        }

        public void Encerrar()
        {
            if (Status == StatusConta.Encerrada)
                throw new InvalidOperationException("A conta já está encerrada.");

            if (Saldo != 0.00m)
                throw new InvalidOperationException("A conta só pode ser encerrada com saldo igual a zero.");

            Status = StatusConta.Encerrada;
        }

        private void GarantirContaOperavel()
        {
            if (Status == StatusConta.Bloqueada)
                throw new InvalidOperationException("A conta está bloqueada e não pode receber movimentações.");

            if (Status == StatusConta.Encerrada)
                throw new InvalidOperationException("A conta está encerrada e não pode receber movimentações.");
        }
    }
}