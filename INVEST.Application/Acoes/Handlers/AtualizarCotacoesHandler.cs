using INVEST.Application.Acoes.Queries;
using INVEST.Application.Shared.Messaging;
using INVEST.Application.Tickers.DTOs;

namespace INVEST.Application.Acoes.Handlers
{

    public class AtualizarCotacoesHandler(IAcaoQuery acaoQuery, IEventPublisher publisher)
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

                var messageId = $"{ticker.Id}-{DateTime.UtcNow:yyyyMMddHHmmss}";

                await publisher.PublishAsync(message: message, exchange: "quotes.exchange", messageId: messageId, ct: ct);
            }
        }
    }
}