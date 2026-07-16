using INVEST.Application.TipoIndicador.DTOs;
using INVEST.Application.TipoIndicador.Repository;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Repositories
{
    public class TipoIndicadorRepository : ITipoIndicadorRepository
    {
        private readonly DataContext _dbContext;

        public TipoIndicadorRepository(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<TipoIndicadorListItemDto?> GetByIdWithQualityParameters(int Id, CancellationToken ct = default)
        {

            return await _dbContext.TiposIndicadores
                .AsNoTracking()
                .Where(List => List.Id == Id)
                .Select(t => new TipoIndicadorListItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Type = t.Type,

                    Qualidades = t.QualidadeIndicadores.Select(q => new QualidadeIndicadorDto
                    {
                        ValorMinimo = q.ValorMinimo,
                        ValorMaximo = q.ValorMaximo,
                        IdNivelQualidade = q.NivelQualidadeId
                    }).ToList()
                })
                .FirstOrDefaultAsync(ct);

        }

        public async Task<List<TipoIndicadorListItemDto>> ListWithQualityParameters(CancellationToken ct = default)
        {
            return await _dbContext.TiposIndicadores
                .AsNoTracking()
                .Select(t => new TipoIndicadorListItemDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Type = t.Type,

                    Qualidades = t.QualidadeIndicadores.Select(q => new QualidadeIndicadorDto
                    {
                        ValorMinimo = q.ValorMinimo,
                        ValorMaximo = q.ValorMaximo,
                        IdNivelQualidade = q.NivelQualidadeId
                    }).ToList()
                })
                .ToListAsync(ct);
        }
    }
}