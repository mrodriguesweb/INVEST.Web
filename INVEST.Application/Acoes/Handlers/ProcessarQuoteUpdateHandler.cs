using INVEST.Application.Acoes.Repository;
using INVEST.Application.Shared;
using INVEST.Application.Shared.Messaging;
using INVEST.Domain.Entities.Acoes;

namespace INVEST.Application.Acoes.Handlers
{
    public class ProcessarQuoteUpdateHandler(IQuoteProvider quoteProvider, IPriceSnapshotRepository snapshotRepository)
    {
        public async Task<decimal> HandleAsync(QuoteUpdateRequestedMessage message, CancellationToken ct = default)
        {
            var price = await quoteProvider.GetQuoteAsync(message.Ticker.Name, ct);

            if (price is null)
            {
                throw new InvalidOperationException($"Cotação não encontrada para {message.Ticker.Name}");
            }

            var snapshot = new PriceSnapshot(
                ticker: message.Ticker.Id,
                price: price.Value,
                capturedAtUtc: DateTime.UtcNow
            );

            await snapshotRepository.AddAsync(snapshot, ct);

            return price.Value;
        }
    }
}
