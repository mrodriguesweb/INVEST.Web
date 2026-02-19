using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Application.Acoes.Abstractions
{
    public interface ICompanyLogoProvider
    {
        Task<CompanyLogo?> GetAsync(string empresa, CancellationToken ct);
    }

    public sealed record CompanyLogo(byte[] Bytes, string ContentType);
}
