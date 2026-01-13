namespace INVEST.Application.Acoes.DTOs
{
    public sealed record DeleteAcaoResult(bool Success, IReadOnlyList<string> Errors)
    {
        public static DeleteAcaoResult Ok() => new(true, Array.Empty<string>());
        public static DeleteAcaoResult Fail(params string[] errors) => new(false, errors);
    }
}
