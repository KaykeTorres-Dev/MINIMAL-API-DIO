using MINIMAL_API___DIO.Domain.Enums;

namespace MINIMAL_API___DIO.Dominio.DTOs
{
    public class AdminDTO
    {
        public string Email { get; set; } = default!;

        public string Senha { get; set; } = default!;

        public Perfil? Perfil { get; set; } = default!;

    }
}
