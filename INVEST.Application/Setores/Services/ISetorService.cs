using INVEST.Application.Setores.DTOs;

namespace INVEST.Application.Setores.Services
{
    public interface ISetorService
    {

        Task<List<SetorListItemDto>> GetList();

    }
}