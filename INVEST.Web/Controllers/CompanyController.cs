using INVEST.Application.Acoes.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace INVEST.Web.Controllers
{
    public class CompanyLogosController(ICompanyLogoProvider logos) : Controller
    {
        [HttpGet("/logos/{empresa}")]
        public async Task<IActionResult> Get(string empresa, CancellationToken ct)
        {
            var normalized = (empresa ?? "").Trim();
            var logo = await logos.GetAsync(normalized, ct);

            if (logo is not null)
            {
                Response.Headers["Cache-Control"] = "public, max-age=86400";
                return File(logo.Bytes, logo.ContentType);
            }

            // fallback gerado (SVG) com iniciais
            var initials = BuildInitials(normalized);
            var svg = $"""
                            <svg xmlns="http://www.w3.org/2000/svg" width="40" height="40">
                              <rect width="100%" height="100%" fill="#e9ecef"/>
                              <text x="50%" y="55%" dominant-baseline="middle" text-anchor="middle"
                                    font-family="Arial" font-size="14" font-weight="600" fill="#495057">
                                {System.Net.WebUtility.HtmlEncode(initials)}
                              </text>
                            </svg>
                            """;

            Response.Headers["Cache-Control"] = "public, max-age=1800";
            return Content(svg, "image/svg+xml");
        }

        private static string BuildInitials(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "?";
            var parts = name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 1) return (parts[0].Length <= 2 ? parts[0] : parts[0][..2]).ToUpperInvariant();
            return $"{char.ToUpperInvariant(parts[0][0])}{char.ToUpperInvariant(parts[^1][0])}";
        }

    }
}