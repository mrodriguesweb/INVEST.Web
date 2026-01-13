using INVEST.Web.ViewModels.AcoesVM;
using System.ComponentModel.DataAnnotations;

namespace INVEST.Web.ViewModels.AcoesVM
{
    public class CreateAcaoViewModel : BaseAcaoViewModel
    {


        [Required(ErrorMessage = "O nome da ação é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ultrapassar 100 caracteres.")]
        public string Name { get; set; } = null!;

    }
}