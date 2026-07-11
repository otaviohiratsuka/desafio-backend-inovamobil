using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.ValueObjects;

namespace PlataformaBancaria.Domain.Repositories
{
    /// <summary>
    /// Contrato do repositório de Conta. Pertence à camada de Domínio,
    /// pois define o comportamento esperado sem depender de detalhes de infraestrutura (EF Core, Postgres, etc.).
    /// </summary>
    public interface IContaRepository
    {
        /// <summary>
        /// Adiciona uma nova conta à base de dados.
        /// </summary>
        Task AdicionarAsync(Conta conta);

        /// <summary>
        /// Atualiza os dados de uma conta já existente.
        /// </summary>
        Task AtualizarAsync(Conta conta);

        /// <summary>
        /// Obtém uma conta a partir do seu identificador único.
        /// Retorna null caso não seja encontrada.
        /// </summary>
        Task<Conta?> ObterPorIdAsync(Guid id);

        /// <summary>
        /// Obtém uma conta a partir do seu Cnpj.
        /// Recebe o Value Object Cnpj (e não uma string "crua"), garantindo que a
        /// validação e o formato do documento já tenham sido assegurados pelo Domínio
        /// antes mesmo de chegar ao repositório.
        /// </summary>
        Task<Conta?> ObterPorCnpjAsync(Cnpj cnpj);
    }
}