namespace PlataformaBancaria.Domain.Repositories
{
    public interface IIdempotenciaRepository
    {
        Task<bool> ExisteAsync(string chave);
        Task AdicionarAsync(string chave);
    }
}