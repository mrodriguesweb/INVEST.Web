using INVEST.Application.Setores.DTOs;

namespace INVEST.Application.Setores.Queries
{
    public interface ISetorQuery
    {

        Task<List<SetorListItemDto>> List();

        Task<bool> Exists(int id);

    }
}
