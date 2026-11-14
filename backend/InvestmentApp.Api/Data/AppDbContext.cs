using InvestmentApp.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace InvestmentApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Investimento> Investimentos => Set<Investimento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(150);
        });

        modelBuilder.Entity<Investimento>(entity =>
        {
            entity.Property(i => i.ValorInicial)
                .HasPrecision(18, 2);

            entity.Property(i => i.ValorTaxa)
                .HasPrecision(18, 2);

            entity.HasOne(i => i.Usuario)
                .WithMany(u => u.Investimentos)
                .HasForeignKey(i => i.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
