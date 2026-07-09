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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conta>(builder =>
            {
                builder.ToTable("Contas");

                builder.HasKey(c => c.Id);

                builder.Property(c => c.Cnpj)
                    .HasConversion(
                        cnpj => (string)cnpj,
                        valor => new Cnpj(valor))
                    .HasColumnName("Cnpj")
                    .IsRequired();

                builder.Property(c => c.RazaoSocial)
                    .IsRequired();

                builder.Property(c => c.Agencia)
                    .IsRequired();

                builder.Property(c => c.Saldo)
                    .HasColumnType("decimal(18,2)");

                builder.Property(c => c.Status)
                    .IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}