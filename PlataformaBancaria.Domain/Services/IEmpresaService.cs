namespace PlataformaBancaria.Domain.Services
{
    public interface IEmpresaService
    {
        Task<string> ObterRazaoSocialPorCnpjAsync(string cnpj);
    }
}