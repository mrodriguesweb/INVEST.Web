//using invesT.Data;
//using invesT.Enums;
//using invesT.Models;
//using Microsoft.EntityFrameworkCore;

//namespace invesT.Services.TipoIndicador.Queries
//{
//    public class TipoIndicadorQuery(DataContext db) : ITipoIndicadorQuery
//    {

//        public async Task<List<TipoIndicadorModel>> List()
//        {

//            var query = await db.TiposIndicadores
//                                .AsNoTracking()
//                                .Select(a => new TipoIndicadorModel()
//                                {
//                                    Id = a.Id,
//                                    Name = a.Name,
//                                    Type = (TypeIndicador)a.Type
//                                })
//                                .ToListAsync();

//            return query;

//        }

//    }
//}