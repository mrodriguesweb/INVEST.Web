namespace INVEST.Domain.Entities.Acoes
{
    public class PriceSnapshot
    {
        public int Id { get; private set; }

        public int TickerId { get; private set; }

        public decimal Price { get; private set; }

        public DateTime CapturedAtUtc { get; private set; }

        public Ticker Ticker { get; set; }

        private PriceSnapshot() { }

        public PriceSnapshot(int ticker, decimal price, DateTime capturedAtUtc)
        {
            TickerId = ticker;
            Price = price;
            CapturedAtUtc = capturedAtUtc;
        }
    }
}
