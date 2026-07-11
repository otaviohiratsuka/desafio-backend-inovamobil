using Microsoft.EntityFrameworkCore;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Domain.ValueObjects;
using PlataformaBancaria.Infrastructure.Data;

namespace PlataformaBancaria.Infrastructure.Repositories
{
    public class ContaRepository : IContaRepository
    {
        private readonly AppDbContext _context;

        public ContaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Conta conta)
        {
            await _context.Contas.AddAsync(conta);
            await _context.SaveChangesAsync();
        }

        public async Task<Conta?> ObterPorIdAsync(Guid id)
        {
            return await _context.Contas
                .Include(c => c.Transacoes)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Conta?> ObterPorCnpjAsync(Cnpj cnpj)
        {
            return await _context.Contas
                .FirstOrDefaultAsync(c => c.Cnpj == cnpj);
        }

        public async Task<IEnumerable<Conta>> ObterTodasAsync()
        {
            return await _context.Contas.ToListAsync();
        }

       public async Task AtualizarAsync(Conta conta)
        {
            // Deixamos o "espião" do EF Core fazer o trabalho dele.
            // Ele vai ver a nova transação na lista e fazer o INSERT no banco automaticamente.
            await _context.SaveChangesAsync();
        }
    }
}