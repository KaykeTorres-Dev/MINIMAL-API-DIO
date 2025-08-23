using MINIMAL_API___DIO.Domain.Enums;

namespace MINIMAL_API___DIO.Domain.ModelViews
{
    public record AdminModelView
    {
        public int Id { get; set; } = default!;

        public string Email { get; set; } = default!;

        public Perfil? Perfil { get; set; } = default!;
    }
}
