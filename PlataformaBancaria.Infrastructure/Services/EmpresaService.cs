using System.Net.Http.Json;
using PlataformaBancaria.Domain.Services;

namespace PlataformaBancaria.Infrastructure.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly HttpClient _httpClient;

        public EmpresaService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> ObterRazaoSocialPorCnpjAsync(string cnpj)
        {
            // A ReceitaWS exige o CNPJ sem pontos ou traços
            var cnpjLimpo = cnpj.Replace(".", "").Replace("/", "").Replace("-", "");
            
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ReceitaWsResponse>($"https://receitaws.com.br/v1/cnpj/{cnpjLimpo}");
                
                // 1. Valida se a Receita Federal realmente encontrou a empresa
                if (response is null || string.IsNullOrWhiteSpace(response.Nome))
                    throw new ArgumentException("CNPJ não encontrado na Receita Federal.");

                // 2. A Regra de Ouro: Bloqueia CNPJs que não estejam ATIVOS
                if (response.Situacao != "ATIVA")
                    throw new InvalidOperationException($"Abertura de conta negada. O CNPJ encontra-se com situação '{response.Situacao}' na Receita Federal.");
                
                return response.Nome;
            }
            catch (HttpRequestException)
            {
                // 3. Se a API estiver fora do ar, abortamos o processo em vez de inventar um nome
                throw new InvalidOperationException("O serviço da Receita Federal está indisponível no momento. Tente novamente mais tarde.");
            }
        }

        // Mapeamos também a Situação que vem no JSON da ReceitaWS
        private class ReceitaWsResponse
        {
            public string Nome { get; set; } = string.Empty;
            public string Situacao { get; set; } = string.Empty;
        }
    }
}