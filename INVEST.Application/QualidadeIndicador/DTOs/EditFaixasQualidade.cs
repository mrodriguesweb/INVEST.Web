namespace INVEST.Application.QualidadeIndicador.DTOs
{
    public sealed record EditFaixasQualidadeCommand(
    int Id,
    IReadOnlyList<QualidadeIndicadorEditCommand> Faixas);

    public sealed record QualidadeIndicadorEditCommand
    (int Id,
    decimal ValorMinimo,
    decimal ValorMaximo);

    public sealed record EditFaixasQualidadeResult(bool Success, int? TipoIndicadorId, IReadOnlyList<string> Errors)
    {
        public static EditFaixasQualidadeResult Ok(int id) => new(true, id, Array.Empty<string>());
        public static EditFaixasQualidadeResult Fail(params string[] errors) => new(false, null, errors);
    }
}