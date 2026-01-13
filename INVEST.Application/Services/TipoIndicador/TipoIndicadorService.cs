//using invesT.Services.AcaoService.Queries;
//using invesT.Services.TipoIndicador.Queries;
//using invesT.ViewModel.AcoesVM;
//using invesT.ViewModel.IndicadoresVM;

//namespace invesT.Services.TipoIndicador
//{
//    public class TipoIndicadorService(ITipoIndicadorQuery tipoIndicadorQuery) : ITipoIndicadorService
//    {

//        public async Task<TipoIndicadorViewModel> PrepareView()
//        {

//            TipoIndicadorViewModel VM = new()
//            {
//                TiposIndic = await tipoIndicadorQuery.List()
//            };

//            return VM;

//        }

//    }
//}