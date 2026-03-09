using INVEST.Application.Tickers.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Application.Acoes.DTOs
{
    public sealed class AcaoItemDto
    {

        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public int IdSetor { get; set; }

        public short AnoEntrada { get; set; }

        public bool Estatal { get; set; }

        public List<TickerItemDto> Tickers { get; set; } = new();

    }
}
