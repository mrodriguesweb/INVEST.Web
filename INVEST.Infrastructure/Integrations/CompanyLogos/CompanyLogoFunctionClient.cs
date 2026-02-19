using INVEST.Application.Acoes.Abstractions;
using System.Net;

namespace INVEST.Infrastructure.Integrations.CompanyLogos
{
    public sealed class CompanyLogoFunctionClient(HttpClient http) : ICompanyLogoProvider
    {
        public async Task<CompanyLogo?> GetAsync(string empresa, CancellationToken ct)
        {
            empresa = (empresa ?? "").Trim().ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(empresa)) return null;

            var url = $"api/GetCompanyLogo?empresa={Uri.EscapeDataString(empresa)}";

            using var resp = await http.GetAsync(url, ct);
            if (resp.StatusCode == HttpStatusCode.NotFound) return null;
            if (!resp.IsSuccessStatusCode) return null;

            var bytes = await resp.Content.ReadAsByteArrayAsync(ct);
            if (bytes.Length == 0) return null;

            var contentType = resp.Content.Headers.ContentType?.MediaType ?? "image/png";
            return new CompanyLogo(bytes, contentType);
        }
    }
}