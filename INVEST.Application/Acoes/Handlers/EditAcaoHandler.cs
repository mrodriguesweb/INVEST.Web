using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Setores.Queries;

namespace INVEST.Application.Acoes.Handlers
{
    public sealed class EditAcaoHandler(IAcaoRepository _acoes, ISetorQuery _setores)
    {

        public async Task<EditAcaoResult> Handle(EditAcaoCommand cmd)
        {

            var errors = new List<string>();

            if (cmd.Id <= 0) errors.Add("AcaoId inválido.");

            var rawTickers = cmd.Tickers ?? new List<string>();
            var tickers = rawTickers
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .Select(t => t.ToUpperInvariant())
                .ToList();

            if (tickers.Count == 0) errors.Add("Informe ao menos 1 ticker válido.");

            if (errors.Count > 0)
                return EditAcaoResult.Fail(errors.ToArray());

            if (!await _setores.Exists(cmd.SetorId))
                return EditAcaoResult.Fail("Setor inválido.");

            var acao = await _acoes.GetByIdWithTickers(cmd.Id);
            if (acao is null)
                return EditAcaoResult.Fail("Ação não encontrada.");

            acao.EditarDados(cmd.AnoEntrada, cmd.Estatal, cmd.SetorId);

            acao.ReplaceTickers(tickers);

            await _acoes.SaveChanges();

            return EditAcaoResult.Ok(acao.Id);

        }
    }
}