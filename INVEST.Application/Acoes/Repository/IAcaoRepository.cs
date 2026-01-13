using INVEST.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace INVEST.Application.Acoes.Repository
{
    public interface IAcaoRepository
    {

        Task Add(Acao acao);

        void Remove(Acao acao);

        Task<Acao?> GetById(int id);

        Task<Acao?> GetByIdWithTickers(int id);

        Task SaveChanges();

    }
}