using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace INVEST.Web.ViewModels.AcoesVM
{
    public class EditAcaoViewModel : BaseAcaoViewModel
    {

        [Required(ErrorMessage = "Selecione uma ação existente.")]
        [Range(1, int.MaxValue, ErrorMessage = "Selecione um ação válida.")]
        public int Id { get; set; }


        [ValidateNever]
        public string Name { get; set; } = null!;

    }
}