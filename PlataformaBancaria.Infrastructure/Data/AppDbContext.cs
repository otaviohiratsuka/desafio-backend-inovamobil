using Microsoft.EntityFrameworkCore;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.ValueObjects;

namespace PlataformaBancaria.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Conta> Contas { get; set; }

        public DbSet<Transacao> Transacoes { get; set; }

        public DbSet<ChaveIdempotencia> ChavesIdempotencia { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChaveIdempotencia>().HasKey(c => c.Id);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}