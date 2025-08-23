using Microsoft.EntityFrameworkCore;
using MINIMAL_API___DIO.Domain.Entities;
using MINIMAL_API___DIO.Dominio.Entidades;

namespace MINIMAL_API___DIO.Infraestrutura.Db
{
    public class DbContexto : DbContext
    {
        // Criando o construtor para receber a configuração do appsettings.json
        private readonly IConfiguration _configurationAppSettings;

        public DbContexto(IConfiguration configurationAppSettings)
        {
            _configurationAppSettings = configurationAppSettings;
        }

        // Criação das tabelas
        public DbSet<Admin> Admins { get; set; } = default!;

        public DbSet<Veiculo> Veiculos { get; set; } = default!;


        // Métodos

        // Método para criar o modelo do banco de dados
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Email = "admin@teste.com",
                    Senha = "123456",
                    Perfil = "Adm"
                }
            );
        }

        // Método sobrescrito para configurar o contexto do banco de dados
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Verifica se o optionsBuilder já foi configurado
            if (!optionsBuilder.IsConfigured)
            {
                var stringConnection = _configurationAppSettings.GetConnectionString("Database")?.ToString();

                if (!string.IsNullOrEmpty(stringConnection))
                {
                    optionsBuilder.UseSqlServer(stringConnection);
                }

            }
        }
    }
}
