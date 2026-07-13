using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.ValueObjects;

namespace PlataformaBancaria.Domain.Repositories
{
    public interface IContaRepository
    {
        Task AdicionarAsync(Conta conta);
        Task AtualizarAsync(Conta conta);
        Task<Conta?> ObterPorIdAsync(Guid id);
        Task<Conta?> ObterPorCnpjAsync(Cnpj cnpj);
        Task<IEnumerable<Conta>> ObterTodasAsync();
    }
}