using INVEST.Domain.Entities;
using INVEST.Domain.Entities.Acoes;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Data
{
    public class DataContext : DbContext
    {

        public DataContext(DbContextOptions<DataContext> opts) : base(opts)
        {

        }

        public DbSet<Acao> Acoes { get; set; }

        public DbSet<Ticker> Tickers { get; set; }

        public DbSet<TipoIndicador> TiposIndicadores { get; set; }

        public DbSet<Setor> Setores { get; set; }

        public DbSet<PriceSnapshot> PriceSnapshots { get; set; }

        public DbSet<Indicador> Indicadores { get; set; }

        public DbSet<QualidadeIndicador> QualidadeIndicadores { get; set; }

        public DbSet<NivelQualidade> NivelQualidade { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Ticker>()
                .HasOne(p => p.Acao)
                .WithMany(c => c.Tickers)
                .HasForeignKey(p => p.AcaoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Acao>()
                .HasOne(p => p.Setor)
                .WithMany(c => c.Acoes)
                .HasForeignKey(p => p.SetorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Indicador>()
                .HasOne(p => p.TipoIndicador)
                .WithMany(c => c.Indicadores)
                .HasForeignKey(p => p.TipoIndicadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Indicador>()
                .HasOne(p => p.Ticker)
                .WithMany(c => c.Indicadores)
                .HasForeignKey(p => p.TickerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QualidadeIndicador>()
                .HasOne(p => p.TipoIndicador)
                .WithMany(c => c.QualidadeIndicadores)
                .HasForeignKey(p => p.TipoIndicadorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<QualidadeSetor>()
                .HasOne(p => p.Setor)
                .WithMany(c => c.QualidadeSetores)
                .HasForeignKey(p => p.SetorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PriceSnapshot>(e =>
            {

                e.HasKey(x => x.Id);

                e.Property(x => x.Price).HasColumnType("numeric(18,4)");

                e.HasOne(p => p.Ticker)
                .WithMany(c => c.PriceSnapshots)
                .HasForeignKey(p => p.TickerId)
                .OnDelete(DeleteBehavior.Cascade);

            });

        }

    }
}