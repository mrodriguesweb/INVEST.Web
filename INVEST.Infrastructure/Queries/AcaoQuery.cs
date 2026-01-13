using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Queries;
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
                              Tickers = a.Tickers.Select(t => t.Name).ToList()
                          })
                          .FirstOrDefaultAsync();

            return query;
        }

        public async Task<List<AcaoListItemDto>> List()
        {

            var query = await db.Acoes
                                .AsNoTracking()
                                .Select(a => new AcaoListItemDto()
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    SetorName = a.Setor.Name
                                })
                                .ToListAsync();

            return query;

        }

    }
}