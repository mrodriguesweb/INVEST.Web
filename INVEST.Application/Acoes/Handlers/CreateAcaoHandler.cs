using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Setores.Queries;
using INVEST.Domain.Entities;

namespace INVEST.Application.Acoes.Handlers
{
    public sealed class CreateAcaoHandler(IAcaoRepository _acoes, ISetorQuery _setores)
    {

        public async Task<CreateAcaoResult> Handle(CreateAcaoCommand cmd)
        {

            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(cmd.Name))
                errors.Add("Name obrigatório.");

            if (cmd.Tickers.Count == 0)
                errors.Add("Informe ao menos 1 ticker.");

            if (errors.Count > 0)
                return CreateAcaoResult.Fail(errors.ToArray());

            if (!await _setores.Exists(cmd.SetorId))
                return CreateAcaoResult.Fail("Setor inválido.");

            if (cmd.Tickers == null)
            {
                errors.Add("Informe ao menos 1 ticker.");
                return CreateAcaoResult.Fail(errors.ToArray());
            }

            var tickers = cmd.Tickers
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (tickers.Count == 0)
                return CreateAcaoResult.Fail("Informe ao menos 1 ticker válido.");

            var acao = new Acao(cmd.Name.Trim(), cmd.AnoEntrada, cmd.Estatal, cmd.SetorId);

            acao.Tickers = tickers
                .Select(t => new Ticker { Name = t.ToUpperInvariant() })
                .ToList();

            await _acoes.Add(acao);
            await _acoes.SaveChanges();

            return CreateAcaoResult.Ok(acao.Id);
        }
    }
}
