using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Queries;
using INVEST.Application.Tickers.DTOs;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Queries
{
    public class AcaoQuery(DataContext db) : IAcaoQuery
    {
        public Task<AcaoItemDto?> GetById(int id)
        {
            var query = db.Acoes
                          .AsNoTracking()
                          .Where(a => a.Id == id)
                          .Select(a => new AcaoItemDto()
                          {
                              Id = a.Id,
                              Name = a.Name,
                              IdSetor = a.SetorId,
                              Estatal = a.Estatal,
                              AnoEntrada = a.AnoEntrada,
                              Tickers = a.Tickers.Select(t => new TickerItemDto
                              {
                                  Id = t.Id,
                                  Name = t.Name
                              }).ToList()
                          })
                          .FirstOrDefaultAsync();

            return query;
        }

        public Task<List<AcaoListItemDto>> List()
        {
            return db.Acoes
                     .AsNoTracking()
                     .Select(a => new AcaoListItemDto
                     {
                         Id = a.Id,
                         Name = a.Name,
                         SetorName = a.Setor.Name,
                         UltimaCotacaoCapturada = a.Tickers
                                              .SelectMany(t => t.PriceSnapshots)
                                              .OrderByDescending(ps => ps.CapturedAtUtc)
                                              .Select(ps => (DateTime?)ps.CapturedAtUtc)
                                              .FirstOrDefault(),
                         Tickers = a.Tickers.Select(t => new TickerItemDto
                         {
                             Id = t.Id,
                             Name = t.Name,
                             LastQuote = t.PriceSnapshots
                                          .OrderByDescending(ps => ps.CapturedAtUtc)
                                          .Select(ps => (decimal?)ps.Price)
                                          .FirstOrDefault(),
                         }).ToList()
                     })
                     .ToListAsync();
        }
    }
}