using Microsoft.EntityFrameworkCore;
using MINIMAL_API___DIO.Domain.Entities;
using MINIMAL_API___DIO.Infraestrutura.Db;
using MINIMAL_API___DIO.Infrastructure.Interfaces;
using System.Linq;

namespace MINIMAL_API___DIO.Domain.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly DbContexto _contexto;

        public VeiculoService(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public List<Veiculo> GetAll(int? page = 1, string? nome = null, string? marca = null)
        {
            // Paginação e filtro por nome
            var query = _contexto.Veiculos.AsQueryable();
            if (!string.IsNullOrEmpty(nome))
            {
                query = query.Where(v => EF.Functions.Like(v.Nome.ToLower(), $"%{nome}%"));
            }

            int itensPerPage = 10;

            if (page != null)
            {
                query = query.Skip(((int) page - 1) * itensPerPage).Take(itensPerPage);
            }
          
            return query.ToList();
        }

        public Veiculo? GetById(int id)
        {
            return _contexto.Veiculos.Where(v => v.Id == id).FirstOrDefault();
        }

        public void Create(Veiculo veiculo)
        {
            _contexto.Veiculos.Add(veiculo);
            _contexto.SaveChanges();
        }

        public void Delete(Veiculo veiculo)
        {
            _contexto.Veiculos.Remove(veiculo);
            _contexto.SaveChanges();
        }

        public void Update(Veiculo veiculo)
        {
            _contexto.Veiculos.Update(veiculo);
            _contexto.SaveChanges();
        }
    }
}
