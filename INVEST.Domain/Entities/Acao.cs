using INVEST.Domain.Entities.Acoes;
using System.ComponentModel.DataAnnotations;

namespace INVEST.Domain.Entities    
{
    public class Acao
    {

        public int Id { get; set; }

        [StringLength(100)]
        public string Name { get; private set; } = null!;

        public short AnoEntrada { get; set; }

        public bool Estatal { get; set; }

        public int SetorId { get; set; }

        public Setor Setor { get; set; }

        public ICollection<Ticker> Tickers { get; set; }

        private Acao() { }

        public Acao(string name, short anoEntrada, bool estatal, int setorId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name obrigatório.", nameof(name));

            Name = name.Trim();
            AnoEntrada = anoEntrada;
            Estatal = estatal;
            SetorId = setorId;

            Tickers = new List<Ticker>();
        }

        public void ReplaceTickers(IEnumerable<string> rawTickers)
        {

            if (rawTickers is null) throw new ArgumentNullException(nameof(rawTickers));

            var normalized = rawTickers
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim().ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (normalized.Count == 0)
                throw new InvalidOperationException("A ação deve ter ao menos 1 ticker.");

            // Remove tickers que não existem mais
            var desired = new HashSet<string>(normalized, StringComparer.OrdinalIgnoreCase);
            var toRemove = Tickers.Where(t => !desired.Contains(t.Name)).ToList();
            foreach (var r in toRemove)
                Tickers.Remove(r);

            // Adiciona novos tickers
            var existing = new HashSet<string>(Tickers.Select(t => t.Name), StringComparer.OrdinalIgnoreCase);
            foreach (var name in normalized)
            {
                if (!existing.Contains(name))
                    Tickers.Add(new Ticker { Name = name });
            }

        }

        public void EditarDados(short anoEntrada, bool estatal, int setorId)
        {
            AnoEntrada = anoEntrada;
            Estatal = estatal;
            SetorId = setorId;
        }

    }
}