using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace PlataformaBancaria.Domain.ValueObjects
{
    public sealed record Cnpj
    {
        public string Numero { get; }

        public Cnpj(string cnpj)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException("O CNPJ não pode ser nulo ou vazio.", nameof(cnpj));

            var numeroLimpo = Regex.Replace(cnpj, @"[^\d]", string.Empty);

            if (numeroLimpo.Length != 14)
                throw new ArgumentException("O CNPJ deve conter exatamente 14 dígitos numéricos.", nameof(cnpj));

            if (numeroLimpo.Distinct().Count() == 1)
                throw new ArgumentException("CNPJ inválido: sequência de dígitos repetidos.", nameof(cnpj));

            if (!ValidarDigitosVerificadores(numeroLimpo))
                throw new ArgumentException("CNPJ inválido: dígitos verificadores não conferem.", nameof(cnpj));

            Numero = numeroLimpo;
        }

        private static bool ValidarDigitosVerificadores(string cnpj)
        {
            int[] multiplicadoresPrimeiroDigito = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int[] multiplicadoresSegundoDigito = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var base12 = cnpj.Substring(0, 12);
            var primeiroDigito = CalcularDigito(base12, multiplicadoresPrimeiroDigito);
            var base13 = base12 + primeiroDigito;
            var segundoDigito = CalcularDigito(base13, multiplicadoresSegundoDigito);

            var digitosCalculados = $"{primeiroDigito}{segundoDigito}";
            var digitosInformados = cnpj.Substring(12, 2);

            return digitosCalculados == digitosInformados;
        }

        private static int CalcularDigito(string baseNumero, int[] multiplicadores)
        {
            var soma = 0;

            for (var i = 0; i < multiplicadores.Length; i++)
                soma += (baseNumero[i] - '0') * multiplicadores[i];

            var resto = soma % 11;

            return resto < 2 ? 0 : 11 - resto;
        }

        public string Formatado()
        {
            return Convert.ToUInt64(Numero).ToString(@"00\.000\.000\/0000-00");
        }

        public static implicit operator string(Cnpj cnpj) => cnpj?.Numero!;

        public static implicit operator Cnpj(string cnpj) => new(cnpj);

        public override string ToString() => Numero;
    }
}