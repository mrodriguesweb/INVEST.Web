using INVEST.Application.Tickers.DTOs;

namespace INVEST.Application.Acoes.DTOs
{
    public sealed class AcaoListItemDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string SetorName { get; set; } = null!;

        public string? LinkLogoEmpresa { get; set; }

        public List<TickerItemDto> Tickers { get; set; } = new List<TickerItemDto>();

        public DateTime? UltimaCotacaoCapturada { get; set; }

    }

}
