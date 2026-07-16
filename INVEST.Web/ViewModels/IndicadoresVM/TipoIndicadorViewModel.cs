namespace INVEST.Web.ViewModels.IndicadoresVM
{
    public class TipoIndicadorViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Em vez de mandar o "short", mandamos o texto amigável para a View
        public string TipoDescricao { get; set; } = string.Empty;

        // Uma lista aninhada com os limites de qualidade
        public List<QualidadeIndicadorViewModel> Qualidades { get; set; } = new();
    }

    public class QualidadeIndicadorViewModel
    {
        public decimal ValorMinimo { get; set; }
        public decimal ValorMaximo { get; set; }
    }
}