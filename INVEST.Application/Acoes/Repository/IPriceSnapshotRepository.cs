using INVEST.Domain.Entities.Acoes;

namespace INVEST.Application.Acoes.Repository
{
    public interface IPriceSnapshotRepository
    {
        Task AddAsync(PriceSnapshot snapshot, CancellationToken ct = default);
    }
}