using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MINIMAL_API___DIO.Domain.DTOs
{
    public record VeiculoDTO
    {
        public string Nome { get; set; } = default!;

        public string Marca { get; set; } = default!;

        public int Ano { get; set; } = default!;
    }
}
