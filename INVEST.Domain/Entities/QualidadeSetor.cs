namespace INVEST.Domain.Entities
{
    public class QualidadeSetor
    {

        public int Id { get; set; }

        public int SetorId { get; set; }

        public int NivelQualidadeId { get; set; }

        public NivelQualidade NivelQualidade { get; set; }

        public Setor Setor { get; set; }

    }
}