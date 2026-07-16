namespace INVEST.Application.TipoIndicador.Queries
{
    public interface ITipoIndicadorQuery
    {

        Task<bool> Exists(int id);

    }
}