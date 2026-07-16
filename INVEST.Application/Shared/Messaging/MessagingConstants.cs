using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Application.Shared.Messaging
{
    public static class MessagingConstants
    {
        // Agrupamos por Exchanges para ficar organizado
        public static class Exchanges
        {
            public const string QuotesUpdateRequested = "quotes.exchange";
            public const string QuotesUpdated = "quotes.updated.exchange";
            public const string DlqExchange = "quotes.exchange.dlx";
        }

        // Agrupamos por Filas
        public static class Queues
        {
            public const string ProcessarQuoteUpdate = "quotes-update";
            public const string ProcessarQuoteUpdateDlq = "quotes-update-dlq";
            public const string NotificarEmail = "quotes-notifications-email";
            public const string AtualizarIndicadores = "quotes-indicators-update";
            public const string AtualizarIndicadoresDlq = "quotes-indicators-update-dlq";
        }
    }
}