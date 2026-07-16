using INVEST.Application.Indicadores.DTOs;
using INVEST.Application.TipoIndicador.DTOs;

namespace INVEST.Application.TipoIndicador.Repository
{
    public interface ITipoIndicadorRepository
    {

        Task<List<TipoIndicadorListItemDto>> ListWithQualityParameters(CancellationToken ct = default);

        Task<TipoIndicadorListItemDto?> GetByIdWithQualityParameters(int Id, CancellationToken ct = default);

    }
}