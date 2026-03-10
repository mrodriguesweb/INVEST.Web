namespace INVEST.Application.Shared
{
    public interface IQuoteProvider
    {

        Task<decimal?> GetQuoteAsync(string ticker, CancellationToken ct = default);

    }
}