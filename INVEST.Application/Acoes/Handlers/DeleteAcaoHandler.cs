using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Queries;
using INVEST.Application.Acoes.Repository;

namespace INVEST.Application.Acoes.Handlers
{
    public sealed class DeleteAcaoHandler(IAcaoRepository _acoes)
    {

        public async Task<DeleteAcaoResult> Handle(int Id)
        {

            if (Id <= 0) return DeleteAcaoResult.Fail("Id inválido.");

            var acao = await _acoes.GetById(Id);
            if (acao is null)
                return DeleteAcaoResult.Fail("Ação não encontrada.");

            _acoes.Remove(acao);

            await _acoes.SaveChanges();

            return DeleteAcaoResult.Ok();

        }
    }
}