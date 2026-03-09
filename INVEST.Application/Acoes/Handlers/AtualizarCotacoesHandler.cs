using INVEST.Application.Acoes.Queries;
using INVEST.Application.Shared.Messaging;
using INVEST.Application.Tickers.DTOs;

namespace INVEST.Application.Acoes.Handlers
{

    public class AtualizarCotacoesHandler(IAcaoQuery acaoQuery, IQuoteUpdatePublisher publisher)
    {
        public async Task HandleAsync(int acaoId, CancellationToken ct = default)
        {

            var acao = await acaoQuery.GetById(acaoId);

            if (acao is null) return;

            foreach (var ticker in acao.Tickers)
            {

                TickerUpdateQuoteDto tickerUpdateQuoteDto = new()
                {
                    Id = ticker.Id,
                    Name = ticker.Name
                };

                var message = new QuoteUpdateRequestedMessage(
                    Ticker: tickerUpdateQuoteDto,
                    RequestedAtUtc: DateTime.UtcNow
                );

                await publisher.PublishAsync(message, ct);
            }
        }
    }
}