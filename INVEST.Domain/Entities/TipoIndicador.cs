using System.ComponentModel.DataAnnotations;

namespace INVEST.Domain.Entities
{
    public class TipoIndicador
    {

        public int Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; } = null!;

        public short Type { get; set; }

        public ICollection<Indicador> Indicadores { get; set; }

        public ICollection<QualidadeIndicador> QualidadeIndicadores { get; set; }

    }
}