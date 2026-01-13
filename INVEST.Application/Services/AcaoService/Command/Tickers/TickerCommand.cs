//using invesT.Data;
//using invesT.Models;

//namespace invesT.Services.AcaoService.Command.Tickers
//{
//    public class TickerCommand(DataContext db) : ITickerCommand
//    {

//        public async Task Create(List<TickerModel> Model, int acaoId)
//        {

//            if (Model.Any())
//            {
//                foreach (TickerModel model in Model)
//                {
//                    var entities = new Ticker()
//                    {
//                        Name = model.Name,
//                        AcaoId = acaoId
//                    };

//                    await db.Tickers.AddRangeAsync(entities);
//                    await db.SaveChangesAsync();

//                }

//            }
//            else
//            {
//                throw new ArgumentNullException(nameof(Model));
//            }

//        }

//    }
//}