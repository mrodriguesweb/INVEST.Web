using INVEST.Application.QualidadeIndicador.DTOs;

namespace INVEST.Application.QualidadeIndicador.Repository
{
    public interface IQualidadeIndicadorRepository
    {

        Task<List<NivelQualidadeItemDto>> List(CancellationToken ct = default);

        Task AddRange(IEnumerable<INVEST.Domain.Entities.QualidadeIndicador> faixas);

        Task SaveChanges();

        Task DeleteByTipoIndicadorIdAsync(int tipoIndicadorId);

    }
}