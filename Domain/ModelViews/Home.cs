namespace MINIMAL_API___DIO.Domain.ModelViews
{
    public struct Home
    {
        public string Message { get => "Bem-vindo a API de veículos - Minimal API"; }

        // Propriedade que retorna a URL da documentação Swagger
        public string SwaggerDoc { get => "/swagger"; }
    }
}
