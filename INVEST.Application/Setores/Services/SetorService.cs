using INVEST.Application.Setores.DTOs;
using INVEST.Application.Setores.Queries;

namespace INVEST.Application.Setores.Services
{
    public class SetorService(ISetorQuery setorQuery) : ISetorService
    {

        public async Task<List<SetorListItemDto>> GetList()
        {

            var setores = await setorQuery.List();

            return setores;

        }

    }
}
