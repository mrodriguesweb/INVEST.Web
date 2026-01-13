using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace INVEST.Web.ViewModels.AcoesVM
{
    public class BaseAcaoViewModel
    {

        [Required(ErrorMessage = "O ano de entrada é obrigatório.")]
        [Range(1900, 2100, ErrorMessage = "Ano deve estar entre 1900 e 2100.")]
        public short AnoEntrada { get; set; }

        public bool Estatal { get; set; }

        [Required(ErrorMessage = "Selecione um setor.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um setor válido.")]
        public int IdSetor { get; set; }

        [Required(ErrorMessage = "Informe os tickers.")]
        [StringLength(20, ErrorMessage = "Ticker não pode ultrapassar 20 caracteres.")]
        public string Tickers { get; set; } = null!;

        [ValidateNever]
        public List<SelectListItem> opcoesSetores { get; set; } = [];

    }
}
