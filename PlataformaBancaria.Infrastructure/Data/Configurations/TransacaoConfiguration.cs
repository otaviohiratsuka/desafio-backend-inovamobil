using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PlataformaBancaria.Domain.Entities;

namespace PlataformaBancaria.Infrastructure.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento da entidade Transacao via Fluent API.
    /// </summary>
    public class TransacaoConfiguration : IEntityTypeConfiguration<Transacao>
    {
        public void Configure(EntityTypeBuilder<Transacao> builder)
        {
            builder.ToTable("Transacoes");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Tipo)
                .HasConversion<string>()
                .IsRequired();

            builder.Property(t => t.Valor)
                .HasColumnType("decimal(18,2)");

            builder.Property(t => t.DataHora)
                .IsRequired();

            builder.Property(t => t.ContaId)
                .IsRequired();

            builder.HasOne<Conta>()
                .WithMany(c => c.Transacoes)
                .HasForeignKey(t => t.ContaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}