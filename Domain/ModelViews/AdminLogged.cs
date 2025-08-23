using MINIMAL_API___DIO.Domain.Enums;

namespace MINIMAL_API___DIO.Domain.ModelViews
{
    public record AdminLogged
    {

        public string Email { get; set; } = default!;

        public string Perfil { get; set; } = default!;

        public string Token { get; set; } = default!;
    }
}
