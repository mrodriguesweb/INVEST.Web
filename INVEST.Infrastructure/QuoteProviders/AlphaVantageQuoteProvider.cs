using INVEST.Application.Shared;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json;

namespace INVEST.Infrastructure.QuoteProviders
{
    public class AlphaVantageQuoteProvider : IQuoteProvider
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public AlphaVantageQuoteProvider(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task<decimal?> GetQuoteAsync(string ticker, CancellationToken ct = default)
        {
            var apiKey = _config["AlphaVantage:ApiKey"];
            var url = $"query?function=GLOBAL_QUOTE&symbol={ticker}.SA&apikey={apiKey}";

            var response = await _httpClient.GetAsync(url, ct);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync(ct);

            using var doc = JsonDocument.Parse(content);

            if (!doc.RootElement.TryGetProperty("Global Quote", out var quote))
            {
                return null;
            }

            if (!quote.TryGetProperty("05. price", out var priceElement))
            {
                return null;
            }

            var raw = priceElement.GetString();

            if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var price))
            {
                return null;
            }

            return price;
        }
    }
}