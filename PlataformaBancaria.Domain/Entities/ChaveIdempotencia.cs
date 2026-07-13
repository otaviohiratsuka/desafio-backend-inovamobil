using System;

namespace PlataformaBancaria.Domain.Entities
{
    public class ChaveIdempotencia
    {
        public string Id { get; private set; }
        public DateTime DataProcessamento { get; private set; }

        protected ChaveIdempotencia() { }

        public ChaveIdempotencia(string id)
        {
            Id = id;
            DataProcessamento = DateTime.UtcNow;
        }
    }
}