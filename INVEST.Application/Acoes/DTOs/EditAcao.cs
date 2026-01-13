namespace INVEST.Application.Acoes.DTOs
{
    public sealed record EditAcaoCommand(
    int Id,
    short AnoEntrada,
    bool Estatal,
    int SetorId,
    IReadOnlyList<string> Tickers);

    public sealed record EditAcaoResult(bool Success, int? AcaoId, IReadOnlyList<string> Errors)
    {
        public static EditAcaoResult Ok(int id) => new(true, id, Array.Empty<string>());
        public static EditAcaoResult Fail(params string[] errors) => new(false, null, errors);
    }
}
