using INVEST.Application.Acoes.DTOs;
using INVEST.Web.ViewModels.AcoesVM;

namespace INVEST.Web.Mappers
{
    public static class AcaoMapper
    {
        public static EditAcaoViewModel MapToViewModel(AcaoItemDto model)
        {
            return new EditAcaoViewModel
            {
                Id = model.Id,
                Name = model.Name,
                AnoEntrada = model.AnoEntrada,
                Estatal = model.Estatal,
                IdSetor = model.IdSetor,
                Tickers = string.Join(",", model.Tickers)
            };
        }
    }

}
