//using invesT.Data;
//using invesT.Models;

//namespace invesT.Services.AcaoService.Command
//{
//    public class AcaoCommand(DataContext db) : IAcaoCommand
//    {

//        public async Task<int> Create(AcaoModel Model)
//        {

//            var entity = new Acao()
//            {
//                Name = Model.Name,
//                AnoEntrada = Model.AnoEntrada,
//                Estatal = Model.Estatal,
//                SetorId = Model.Setor.Id,
//            };

//            await db.Acoes.AddAsync(entity);
//            await db.SaveChangesAsync();

//            return entity.Id;

//        }

//    }
//}