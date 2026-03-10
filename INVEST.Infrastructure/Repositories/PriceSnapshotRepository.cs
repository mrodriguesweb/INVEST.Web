using INVEST.Application.Acoes.Repository;
using INVEST.Domain.Entities.Acoes;
using INVEST.Infrastructure.Data;

namespace INVEST.Infrastructure.Repositories
{
    public class PriceSnapshotRepository(DataContext context) : IPriceSnapshotRepository
    {
        public async Task AddAsync(PriceSnapshot snapshot, CancellationToken ct = default)
        {
            context.PriceSnapshots.Add(snapshot);
            await context.SaveChangesAsync(ct);
        }
    }
}
