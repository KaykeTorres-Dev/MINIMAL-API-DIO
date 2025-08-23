using MINIMAL_API___DIO.Dominio.DTOs;
using MINIMAL_API___DIO.Dominio.Entidades;

namespace MINIMAL_API___DIO.Infrastructure.Interfaces
{
    public interface IAdminService
    {
        Admin? Login(LoginDTO loginDTO);
        Admin Create(Admin admin);
        Admin? GetById(int id);
        List<Admin> GetAll(int? page);
    }
}
