using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaBancaria.Domain.Entities;
using PlataformaBancaria.Domain.ValueObjects;

namespace PlataformaBancaria.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento da entidade Conta via Fluent API.
    /// </summary>
    public class ContaConfiguration : IEntityTypeConfiguration<Conta>
    {
        public void Configure(EntityTypeBuilder<Conta> builder)
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

            builder.HasMany(c => c.Transacoes)
                .WithOne()
                .HasForeignKey(t => t.ContaId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Metadata.FindNavigation(nameof(Conta.Transacoes))
                ?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}