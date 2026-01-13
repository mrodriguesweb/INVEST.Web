using INVEST.Application.Acoes.Repository;
using INVEST.Domain.Entities;
using INVEST.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace INVEST.Infrastructure.Repositories
{
    public sealed class AcaoRepository(DataContext _db) : IAcaoRepository
    {

        public Task Add(Acao acao) => _db.Acoes.AddAsync(acao).AsTask();

        public Task<Acao?> GetById(int id) => _db.Acoes.FirstOrDefaultAsync(a => a.Id == id);

        public Task<Acao?> GetByIdWithTickers(int id) => _db.Acoes.Include(a => a.Tickers).FirstOrDefaultAsync(a => a.Id == id);

        public void Remove(Acao acao) => _db.Acoes.Remove(acao);

        public Task SaveChanges() => _db.SaveChangesAsync();

    }
}
