using Azure.Messaging.ServiceBus;
using INVEST.Application.Acoes.Handlers;
using INVEST.Application.Shared.Messaging;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Company.Functions
{
    public class ProcessarQuoteUpdateFunction(ProcessarQuoteUpdateHandler handler, ILogger<ProcessarQuoteUpdateFunction> logger)
    {
        [Function(nameof(ProcessarQuoteUpdateFunction))]
        public async Task Run([ServiceBusTrigger("quotes-update", Connection = "SERVICE_BUS", AutoCompleteMessages = false)]
        ServiceBusReceivedMessage message, ServiceBusMessageActions messageActions, CancellationToken ct)
        {
            logger.LogInformation(
                "Processando mensagem {MessageId} | DeliveryCount: {DeliveryCount}",
                message.MessageId, message.DeliveryCount);

            var payload = JsonSerializer.Deserialize<QuoteUpdateRequestedMessage>(
                message.Body.ToString());

            if (payload is null)
            {
                logger.LogError("Payload inválido. Enviando para DLQ.");

                await messageActions.DeadLetterMessageAsync(message,
                    deadLetterReason: "PayloadInvalido",
                    deadLetterErrorDescription: "Não foi possível desserializar a mensagem.",
                    cancellationToken: ct);

                return;
            }

            try
            {
                await handler.HandleAsync(payload, ct);

                await messageActions.CompleteMessageAsync(message, ct);

                logger.LogInformation(
                    "Cotação de {Ticker} processada com sucesso.", payload.Ticker);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex,
                    "Falha ao processar {Ticker}. DeliveryCount: {Count}. Será tentado novamente.",
                    payload.Ticker, message.DeliveryCount);

                await messageActions.AbandonMessageAsync(message, cancellationToken: ct);
            }
        }
    }
}
