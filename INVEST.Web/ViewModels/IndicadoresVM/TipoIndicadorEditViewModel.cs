using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace INVEST.Web.ViewModels.IndicadoresVM
{
    public class TipoIndicadorEditViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        // Lista das faixas que o usuário está editando
        public List<QualidadeIndicadorEditViewModel> Qualidades { get; set; } = new();

        // Lista para popular o <select> no HTML
        public IEnumerable<SelectListItem> NiveisDisponiveis { get; set; } = new List<SelectListItem>();
    }

    public class QualidadeIndicadorEditViewModel
    {
        [Required(ErrorMessage = "O Nível é obrigatório.")]
        public int NivelQualidadeId { get; set; }

        [Required]
        public decimal ValorMinimo { get; set; }

        [Required]
        public decimal ValorMaximo { get; set; }
    }
}