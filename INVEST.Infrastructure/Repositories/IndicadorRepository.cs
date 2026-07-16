using INVEST.Application.Indicadores.DTOs;
using INVEST.Application.Indicadores.Repository;
using INVEST.Domain.Entities;
using INVEST.Domain.Enums;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Repositories
{
    public sealed class IndicadorRepository(DataContext _db) : IIndicadorRepository
    {

        public async Task AddRange(IEnumerable<Indicador> indicadores, CancellationToken ct = default)
        {
            await _db.Indicadores.AddRangeAsync(indicadores, ct);
            await _db.SaveChangesAsync(ct);
        }
    

        public async Task<List<TickerIndicadoresDto>> ListIndicadoresWithTickers(CancellationToken ct = default)
        {
            var radar = await _db.Tickers
                .AsNoTracking() // Fundamental para consultas apenas de leitura
                .Select(t => new TickerIndicadoresDto
                {
                    Ticker = t.Name,

                    // Navega na relação Acao -> Setor para pegar o nome
                    Setor = t.Acao.Setor.Name,

                    HistoricoIndicadores = new HistoricoIndicadoresDto
                    {
                        // Subconsultas otimizadas: Busca apenas o valor mais recente de cada tipo
                        Ebitda = t.Indicadores
                            .Where(i => i.TipoIndicadorId == (int)TipoIndicadorEnum.Ebitda)
                            .OrderByDescending(i => i.DataRegistro)
                            .Select(i => i.ValorDecimal ?? 0)
                            .FirstOrDefault(),

                        Roe = t.Indicadores
                            .Where(i => i.TipoIndicadorId == (int)TipoIndicadorEnum.Roe)
                            .OrderByDescending(i => i.DataRegistro)
                            .Select(i => i.ValorDecimal ?? 0)
                            .FirstOrDefault(),

                        MargemLiquida = t.Indicadores
                            .Where(i => i.TipoIndicadorId == (int)TipoIndicadorEnum.MargemLiquida)
                            .OrderByDescending(i => i.DataRegistro)
                            .Select(i => i.ValorDecimal ?? 0)
                            .FirstOrDefault(),
                    }
                })
                .ToListAsync(ct);

            return radar;
        }

    }
}