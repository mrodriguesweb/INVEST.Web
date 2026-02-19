using INVEST.Application.Acoes.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace INVEST.Infrastructure.Integrations.CompanyLogos
{

    public sealed record LogoCacheItem(bool Found, CompanyLogo? Logo);

    public sealed class CachedCompanyLogoProvider(ICompanyLogoProvider inner, IMemoryCache cache) : ICompanyLogoProvider
    {
        public async Task<CompanyLogo?> GetAsync(string empresa, CancellationToken ct)
        {
            var normalized = (empresa ?? "").Trim().ToUpperInvariant();
            var key = $"logo:{normalized}";

            var item = await cache.GetOrCreateAsync(key, async entry =>
            {
                var logo = await inner.GetAsync(normalized, ct);

                if (logo is null)
                {
                    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30);
                    entry.SlidingExpiration = null;
                    return new LogoCacheItem(false, null);
                }

                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(7);
                entry.SlidingExpiration = TimeSpan.FromHours(12);
                return new LogoCacheItem(true, logo);
            });

            return item!.Found ? item.Logo : null;
        }
    }

}