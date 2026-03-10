using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Queries;

namespace INVEST.Application.Acoes.Services
{
    public class AcaoService(IAcaoQuery acaoQuery) : IAcaoService
    {

        public async Task<List<AcaoListItemDto>> GetList()
        {

            var acoes = await acaoQuery.List();

            foreach (var acao in acoes)
            {
                string EmpresaName = (acao.Name ?? string.Empty).ToUpper().Trim();

                acao.LinkLogoEmpresa = $"/logos/{EmpresaName}";
            }

            return acoes;

        }

    }
}