using System;

namespace PlataformaBancaria.Domain.Entities
{
    public class ChaveIdempotencia
    {
        public string Id { get; private set; } // A própria string da chave será o ID no banco
        public DateTime DataProcessamento { get; private set; }

        protected ChaveIdempotencia() { } // Construtor vazio exigido pelo EF Core

        public ChaveIdempotencia(string id)
        {
            Id = id;
            DataProcessamento = DateTime.UtcNow;
        }
    }
}