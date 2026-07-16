using INVEST.Application.Indicadores.DTOs;
using INVEST.Domain.Entities;

namespace INVEST.Application.Indicadores.Repository
{
    public interface IIndicadorRepository
    {

        Task AddRange(IEnumerable<Indicador> indicadores, CancellationToken ct = default);

        Task<List<TickerIndicadoresDto>> ListIndicadoresWithTickers(CancellationToken ct = default);

    }
}
