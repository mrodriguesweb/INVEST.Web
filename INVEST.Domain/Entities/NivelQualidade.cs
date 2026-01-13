using System.ComponentModel.DataAnnotations;

namespace INVEST.Domain.Entities
{
    public class NivelQualidade
    {

        public int Id { get; set; }

        [StringLength(30)]
        public string Nome { get; set; } = null!;

    }
}