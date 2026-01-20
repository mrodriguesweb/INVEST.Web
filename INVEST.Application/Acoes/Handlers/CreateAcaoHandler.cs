using INVEST.Application.Acoes.DTOs;
using INVEST.Application.Acoes.Repository;
using INVEST.Application.Common.Errors;
using INVEST.Application.Setores.Queries;
using INVEST.Domain.Entities;

namespace INVEST.Application.Acoes.Handlers
{
    public sealed class CreateAcaoHandler(IAcaoRepository _acoes, ISetorQuery _setores)
    {

        public async Task<Result<int>> Handle(CreateAcaoCommand cmd)
        {

            var errors = new List<Error>();

            if (string.IsNullOrWhiteSpace(cmd.Name))
                errors.Add(new Error(ErrorType.Validation, "Name obrigatório."));

            if (cmd.Tickers == null)
            {
                errors.Add(new Error(ErrorType.Validation, "Informe ao menos 1 ticker."));
                return Result<int>.Fail(errors.ToArray());
            }

            if (cmd.Tickers.Count == 0)
                errors.Add(new Error(ErrorType.Validation, "Informe ao menos 1 ticker."));

            if (errors.Count > 0)
                return Result<int>.Fail(errors.ToArray());

            if (!await _setores.Exists(cmd.SetorId))
                return Result<int>.Fail(new Error(ErrorType.Validation, "Setor inválido."));

            var tickers = cmd.Tickers
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (tickers.Count == 0)
                return Result<int>.Fail(new Error(ErrorType.Validation, "Informe ao menos 1 ticker válido."));

            var acao = new Acao(cmd.Name.Trim(), cmd.AnoEntrada, cmd.Estatal, cmd.SetorId);

            acao.Tickers = tickers
                .Select(t => new Ticker { Name = t.ToUpperInvariant() })
                .ToList();

            await _acoes.Add(acao);
            await _acoes.SaveChanges();

            return Result<int>.Ok(acao.Id);
        }
    }
}
