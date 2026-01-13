using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Queries;

namespace INVEST.Application.Acoes.Services
{
    public class AcaoService(IAcaoQuery acaoQuery) : IAcaoService
    {

        public async Task<List<AcaoListItemDto>> GetList()
        {

            var acoes = await acaoQuery.List();

            return acoes;

        }

        //public async Task SubmitCreate(CreateAcaoViewModel VM)
        //{

        //    AcaoModel Model = AcaoMapper.MapToModel(VM);

        //    using var transaction = await db.Database.BeginTransactionAsync();

        //    try
        //    {

        //        int idNewAcao = await acaoCommand.Create(Model);

        //        await tickerCommand.Create(Model.Tickers, idNewAcao);

        //        await transaction.CommitAsync();

        //    }
        //    catch (Exception)
        //    {
        //        await transaction.RollbackAsync();
        //        throw;
        //    }
        //}

    }
}