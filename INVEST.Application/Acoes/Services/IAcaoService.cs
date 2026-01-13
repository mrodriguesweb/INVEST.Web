using INVEST.Application.Acoes.DTOs;

namespace INVEST.Application.Acoes.Services
{
    public interface IAcaoService
    {

        Task<List<AcaoListItemDto>> GetList();

        //Task<CreateAcaoViewModel> PrepareCreate();

        //Task SubmitCreate(CreateAcaoViewModel VM);

    }
}