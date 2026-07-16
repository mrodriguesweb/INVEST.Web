namespace INVEST.Application.TipoIndicador.DTOs
{
    public record TipoIndicadorListItemDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public short Type { get; init; }

        // A lista aninhada com as qualidades daquele indicador
        public List<QualidadeIndicadorDto> Qualidades { get; init; } = new();
    }

    public record QualidadeIndicadorDto
    {
        public decimal ValorMinimo { get; init; }

        public decimal ValorMaximo { get; init; }

        public int IdNivelQualidade { get; init; }
    }
}