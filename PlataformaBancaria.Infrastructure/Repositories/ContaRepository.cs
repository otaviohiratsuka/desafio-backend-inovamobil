using Microsoft.EntityFrameworkCore;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Domain.ValueObjects;
using PlataformaBancaria.Infrastructure.Data;

namespace PlataformaBancaria.Infrastructure.Repositories
{
    /// <summary>
    /// Implementação concreta do repositório de Conta. Pertence à camada de Infraestrutura,
    /// pois é aqui que residem os detalhes técnicos de persistência (EF Core).
    /// </summary>
    public class ContaRepository : IContaRepository
    {
        private readonly AppDbContext _context;

        public ContaRepository(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adiciona a conta ao contexto e persiste imediatamente no banco.
        /// </summary>
        public async Task AdicionarAsync(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Marca a conta como modificada e persiste as alterações no banco.
        /// Como a entidade normalmente já está sendo rastreada pelo EF Core
        /// (obtida via ObterPorIdAsync no mesmo escopo de contexto), o Update
        /// garante a atualização também em cenários onde a entidade chega desanexada.
        /// </summary>
        public async Task AtualizarAsync(Conta conta)
        {
            _context.Contas.Update(conta);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Busca uma conta pelo Id. Retorna null se não encontrada.
        /// </summary>
        public async Task<Conta?> ObterPorIdAsync(Guid id)
        {
            return await _context.Contas
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        /// <summary>
        /// Busca uma conta pelo Cnpj. Como a propriedade Cnpj possui HasConversion
        /// configurada no AppDbContext (Cnpj -> string), o EF Core traduz a comparação
        /// automaticamente para uma consulta sobre a coluna string no banco.
        /// </summary>
        public async Task<Conta?> ObterPorCnpjAsync(Cnpj cnpj)
        {
            return await _context.Contas
                .FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        }
    }
}