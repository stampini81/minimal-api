using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbSet<Veiculo> Veiculos { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador {
                Id = 1,
                Email = "administrador@teste.com",
                Senha = "123456",
                Perfil = "Adm"
             }
        );

        modelBuilder.Entity<Veiculo>().HasData(
            new Veiculo { Id = 1, Nome = "Fusca", Marca = "Volkswagen", Ano = 1980 },
            new Veiculo { Id = 2, Nome = "Gol", Marca = "Volkswagen", Ano = 2005 },
            new Veiculo { Id = 3, Nome = "Uno", Marca = "Fiat", Ano = 1998 }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if(!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("MySql")?.ToString();
            if(!string.IsNullOrEmpty(stringConexao) && !string.Equals(stringConexao, "InMemory", StringComparison.OrdinalIgnoreCase))
            {
                optionsBuilder.UseMySql(
                    stringConexao,
                    ServerVersion.AutoDetect(stringConexao)
                );
            }
        }
    }
}