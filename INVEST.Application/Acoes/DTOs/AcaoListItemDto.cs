namespace INVEST.Application.Acoes.DTOs
{
    public sealed class AcaoListItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string SetorName { get; set; } = null!;
    }

}
