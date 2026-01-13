using System.ComponentModel.DataAnnotations;

namespace INVEST.Domain.Entities
{
    public class Ticker
    {

        public int Id { get; set; }

        [StringLength(5)]
        public string Name { get; set; } = null!;

        public int AcaoId { get; set; }

        public Acao Acao { get; set; }

        public ICollection<Indicador> Indicadores { get; set; }


    }
}