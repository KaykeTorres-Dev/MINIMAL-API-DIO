using MINIMAL_API___DIO.Domain.Entities;

namespace MINIMAL_API___DIO.Infrastructure.Interfaces
{
    public interface IVeiculoService
    {
        List<Veiculo> GetAll(int? page = 1, string? nome = null, string? marca = null);

        Veiculo? GetById(int id);

        void Create(Veiculo veiculo);

        void Update(Veiculo veiculo);

        void Delete(Veiculo veiculo);

    }
}
