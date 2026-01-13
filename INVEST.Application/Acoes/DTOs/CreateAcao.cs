namespace INVEST.Application.Acoes.DTOs
{
    public sealed record CreateAcaoCommand(
    string Name,
    short AnoEntrada,
    bool Estatal,
    int SetorId,
    IReadOnlyList<string> Tickers);

    public sealed record CreateAcaoResult(bool Success, int? AcaoId, IReadOnlyList<string> Errors)
    {
        public static CreateAcaoResult Ok(int id) => new(true, id, Array.Empty<string>());
        public static CreateAcaoResult Fail(params string[] errors) => new(false, null, errors);
    }

}
