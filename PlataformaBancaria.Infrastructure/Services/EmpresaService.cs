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
                // Fazemos a chamada HTTP para a API pública
                var response = await _httpClient.GetFromJsonAsync<ReceitaWsResponse>($"https://receitaws.com.br/v1/cnpj/{cnpjLimpo}");
                
                // Se encontrar o nome, retorna. Se o JSON vier vazio, retorna um aviso.
                return response?.Nome ?? "Razão Social Não Encontrada";
            }
            catch
            {
                // Se a internet cair ou a API da Receita estiver fora do ar, não travamos o banco:
                return "Empresa Padrão (Serviço Indisponível)";
            }
        }

        // Usamos um DTO privado apenas para mapear o campo "nome" que a ReceitaWS devolve no JSON
        private class ReceitaWsResponse
        {
            public string Nome { get; set; }
        }
    }
}