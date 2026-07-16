using INVEST.Application.Indicadores.Abstractions;
using INVEST.Application.Indicadores.DTOs;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace INVEST.Infrastructure.Integrations.MarketData
{
    /// <summary>
    /// Cliente HTTP responsável por extrair dados fundamentalistas de portais financeiros.
    /// ATENÇÃO: Esta integração utiliza engenharia reversa de uma API não documentada (Scraping).
    /// Altamente sensível a mudanças na estrutura do DOM e do JSON de retorno do provedor.
    /// </summary>
    public class MarketDataProviderClient : IIndicadoresMarketClient
    {
        private readonly HttpClient _httpClient;

        // Injetamos o IConfiguration para ler do Docker Compose / appsettings
        public MarketDataProviderClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            // Busca a URL do compose. O padrão __ no compose vira : no C#
            var baseUrl = configuration["MarketData:BaseUrl"]
                          ?? throw new ArgumentException("A URL base do MarketData não foi configurada.");

            // Configura a URL Base. Toda requisição agora partirá daqui!
            _httpClient.BaseAddress = new Uri(baseUrl);

            _httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("Accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");

            // Usa a variável em vez de hardcode no Referer para simular navegação orgânica
            _httpClient.DefaultRequestHeaders.Add("Referer", baseUrl);
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", "pt-BR,pt;q=0.9,en-US;q=0.8,en;q=0.7");
        }

        /// <summary>
        /// Busca o ID interno da empresa via HTML e realiza a extração do JSON com o histórico de balanços.
        /// </summary>
        /// <param name="ticker">O código da ação na B3 (ex: VALE3).</param>
        /// <param name="ct">Token para cancelamento assíncrono.</param>
        /// <returns>DTO contendo os indicadores extraídos (EBITDA, ROE, Margem Líquida).</returns>
        /// <exception cref="InvalidOperationException">Lançada caso o ticker não seja encontrado no provedor.</exception>
        public async Task<HistoricoIndicadoresDto> ExtrairIndicadoresAsync(string ticker, CancellationToken ct)
        {
            var companyId = await ObterIdInternoAsync(ticker, ct);

            if (string.IsNullOrEmpty(companyId))
                throw new InvalidOperationException($"Não foi possível encontrar o ID interno para o ticker {ticker}.");

            // Usamos apenas o caminho relativo
            var apiUrl = $"api/balancos/balancoresultados/chart/{companyId}/5/yearly/";

            var response = await _httpClient.GetAsync(apiUrl, ct);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(ct);

            return ParseTabelaInvestidor10(json);
        }

        private async Task<string> ObterIdInternoAsync(string ticker, CancellationToken ct)
        {
            // Caminho relativo, o HttpClient junta automaticamente com o BaseAddress
            var urlHtml = $"acoes/{ticker.ToLower()}/";
            var response = await _httpClient.GetAsync(urlHtml, ct);

            if (!response.IsSuccessStatusCode) return string.Empty;

            var html = await response.Content.ReadAsStringAsync(ct);

            var regex = new Regex(@"data-company-id=[\""](\d+)[\""]", RegexOptions.IgnoreCase);
            var match = regex.Match(html);

            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            var regexJs = new Regex(@"companyId\s*=\s*(\d+)", RegexOptions.IgnoreCase);
            var matchJs = regexJs.Match(html);

            return matchJs.Success ? matchJs.Groups[1].Value : string.Empty;
        }

        private HistoricoIndicadoresDto ParseTabelaInvestidor10(string jsonString)
        {
            using var doc = JsonDocument.Parse(jsonString);
            var root = doc.RootElement;

            var dto = new HistoricoIndicadoresDto();

            // O root é um Array de Arrays
            foreach (var linha in root.EnumerateArray())
            {
                // Ignora a primeira linha (cabeçalho) ou linhas vazias
                if (linha.GetArrayLength() == 0 || linha[0].GetString() == "#")
                    continue;

                var nomeIndicador = linha[0].GetString();

                // O índice 1 sempre é a coluna "ÚLT. 12M". 
                // O índice 2 seria o ano anterior, o 3 outro ano, etc.
                var valorCelula = ExtrairValorDaCelula(linha[1]);

                // Mapeia de acordo com o nome exato que vem no JSON
                dto = nomeIndicador switch
                {
                    "Margem Ebitda - (%)" => dto with { Ebitda = ConverterParaDecimal(valorCelula) },
                    "ROE - (%)" => dto with { Roe = ConverterParaDecimal(valorCelula) },
                    "Margem Líquida - (%)" => dto with { MargemLiquida = ConverterParaDecimal(valorCelula) },
                    _ => dto // Ignora linhas que não nos interessam
                };
            }

            return dto;
        }

        // Como as células podem ser uma String "35,00%" ou um Array ["214 Bilhões", "R$ 214..."]
        // precisamos testar o tipo antes de ler.
        private string ExtrairValorDaCelula(JsonElement celula)
        {
            if (celula.ValueKind == JsonValueKind.String)
            {
                return celula.GetString() ?? "";
            }

            if (celula.ValueKind == JsonValueKind.Array && celula.GetArrayLength() > 0)
            {
                // Pega o primeiro item do array ("214,86 Bilhões")
                return celula[0].GetString() ?? "";
            }

            return "";
        }

        // Limpa a sujeira do texto e converte para número
        private decimal ConverterParaDecimal(string valorSujo)
        {
            if (string.IsNullOrWhiteSpace(valorSujo) || valorSujo == "-")
                return 0;

            // Remove os símbolos e espaços
            var limpo = valorSujo
                .Replace("%", "")
                .Replace("R$", "")
                .Replace("Bilhões", "")
                .Replace("Milhões", "")
                .Trim();

            // Usa a cultura PT-BR para entender que a vírgula é o separador decimal
            if (decimal.TryParse(limpo, NumberStyles.Any, new CultureInfo("pt-BR"), out var resultado))
            {
                // Ajuste de grandeza: se vinha escrito "Bilhões", multiplica por 1 bilhão
                if (valorSujo.Contains("Bilhões")) return resultado * 1_000_000_000m;
                if (valorSujo.Contains("Milhões")) return resultado * 1_000_000m;

                return resultado;
            }

            return 0;
        }
    }
}