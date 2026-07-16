using INVEST.Application.Indicadores.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Application.Indicadores.Abstractions
{
    public interface IIndicadoresMarketClient
    {
        Task<HistoricoIndicadoresDto> ExtrairIndicadoresAsync(string ticker, CancellationToken ct);
    }
}