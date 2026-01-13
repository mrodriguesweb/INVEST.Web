using INVEST.Application.Setores.DTOs;
using INVEST.Application.Setores.Queries;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Queries
{
    public class SetorQuery(DataContext db) : ISetorQuery
    {
        public Task<bool> Exists(int id)
        {
            
            var query = db.Setores
                          .AsNoTracking()
                          .AnyAsync(s => s.Id == id);

            return query;

        }

        public async Task<List<SetorListItemDto>> List()
        {

            var query = await db.Setores
                                .AsNoTracking()
                                .Select(s => new SetorListItemDto()
                                {
                                    Id = s.Id,
                                    Name = s.Name,
                                })
                                .OrderBy(s => s.Name)
                                .ToListAsync();

            return query;

        }

    }
}