using INVEST.Application.Acoes.DTOs;

namespace INVEST.Application.Acoes.Queries
{
    public interface IAcaoQuery
    {

        Task<List<AcaoListItemDto>> List();

        Task<AcaoItemDto?> GetById(int id);

    }
}