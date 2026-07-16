using INVEST.Application.QualidadeIndicador.DTOs;
using INVEST.Application.QualidadeIndicador.Repository;
using INVEST.Domain.Entities;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Repositories
{
    public class QualidadeIndicadorRepository(DataContext db) : IQualidadeIndicadorRepository
    {
        public async Task AddRange(IEnumerable<QualidadeIndicador> faixas)
        {
            await db.QualidadeIndicadores.AddRangeAsync(faixas);
            await db.SaveChangesAsync();
        }

        public async Task<List<NivelQualidadeItemDto>> List(CancellationToken ct = default)
        {
            
            var query = await db.NivelQualidade
                                .AsNoTracking()
                                .Select(nq => new NivelQualidadeItemDto
                                {
                                    Id = nq.Id,
                                    Name = nq.Nome
                                })
                                .ToListAsync(ct);

            return query;

        }

        public async Task DeleteByTipoIndicadorIdAsync(int tipoIndicadorId)
        {
            await db.QualidadeIndicadores
                .Where(q => q.TipoIndicadorId == tipoIndicadorId)
                .ExecuteDeleteAsync();
        }

        public Task SaveChanges() => db.SaveChangesAsync();
    }
}