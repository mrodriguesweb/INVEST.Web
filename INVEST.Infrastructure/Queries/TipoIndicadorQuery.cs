using INVEST.Application.TipoIndicador.Queries;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Infrastructure.Queries
{
    public class TipoIndicadorQuery(DataContext db) : ITipoIndicadorQuery
    {
        public Task<bool> Exists(int id)
        {
            var query = db.TiposIndicadores
                          .AsNoTracking()
                          .AnyAsync(s => s.Id == id);

            return query;
        }
    }
}
