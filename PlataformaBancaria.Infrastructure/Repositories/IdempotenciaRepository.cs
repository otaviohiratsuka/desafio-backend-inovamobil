using Microsoft.EntityFrameworkCore;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.Repositories;
using PlataformaBancaria.Infrastructure.Data;

namespace PlataformaBancaria.Infrastructure.Repositories
{
    public class IdempotenciaRepository : IIdempotenciaRepository
    {
        private readonly AppDbContext _context;

        public IdempotenciaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExisteAsync(string chave)
        {
            return await _context.ChavesIdempotencia.AnyAsync(c => c.Id == chave);
        }

        public async Task AdicionarAsync(string chave)
        {
            _context.ChavesIdempotencia.Add(new ChaveIdempotencia(chave));
            await _context.SaveChangesAsync();
        }
    }
}