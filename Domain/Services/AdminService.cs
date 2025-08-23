using Microsoft.EntityFrameworkCore;
using MINIMAL_API___DIO.Domain.ModelViews;
using MINIMAL_API___DIO.Dominio.DTOs;
using MINIMAL_API___DIO.Dominio.Entidades;
using MINIMAL_API___DIO.Infraestrutura.Db;
using MINIMAL_API___DIO.Infrastructure.Interfaces;

namespace MINIMAL_API___DIO.Dominio.Servicos
{
    public class AdminService : IAdminService
    {
        private readonly DbContexto _contexto;
        public AdminService(DbContexto contexto)
        {
            _contexto = contexto;
        }

        public Admin Create(Admin admin)
        {
            _contexto.Admins.Add(admin);
            _contexto.SaveChanges();

            return admin;
        }

        public List<Admin> GetAll(int? page)
        {
            var query = _contexto.Admins.AsQueryable();

            int itensPerPage = 10;

            if (page != null)
            {
                query = query.Skip(((int)page - 1) * itensPerPage).Take(itensPerPage);
            }

            return query.ToList();
        }

        public Admin? GetById(int id)
        {
            return _contexto.Admins.Where(a => a.Id == id).FirstOrDefault();
        }

        // Método para realizar o login do administrador
        public Admin? Login(LoginDTO loginDTO)
        {
            var adm = _contexto.Admins.Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
            return adm;
        }
    }
}
