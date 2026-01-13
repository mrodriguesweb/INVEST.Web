using System.ComponentModel.DataAnnotations;

namespace INVEST.Domain.Entities
{
    public class Setor
    {

        public int Id { get; set; }

        [StringLength(30)]
        public string Name { get; set; } = null!;

        public ICollection<Acao> Acoes { get; set; }

        public ICollection<QualidadeSetor> QualidadeSetores { get; set; }

    }
}