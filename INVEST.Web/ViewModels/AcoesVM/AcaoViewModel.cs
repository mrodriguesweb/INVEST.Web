namespace INVEST.Web.ViewModels.AcoesVM
{
    public class AcaoViewModel
    {

        public int Id { get; set; }

        public string? LinkLogoEmpresa { get; set; }

        public string Name { get; set; } = null!;

        public string SetorName { get; set; } = null!;

        public string? Cotacoes { get; set; }

        public DateTime? UltimaCotacaoCapturada { get; set; }

    }
}