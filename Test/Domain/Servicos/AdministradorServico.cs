using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Servicos;
using MinimalApi.Infraestrutura.Db;

namespace Test.Domain.Entidades;

[TestClass]
public class AdministradorServicoTest
{
    private DbContexto CriarContextoDeTeste()
    {
        // Usa EF InMemory para evitar dependência de MySQL nos testes de unidade
        var inMemorySettings = new Dictionary<string, string?>
        {
            { "ConnectionStrings:MySql", "InMemory" },
            { "Jwt", "minimal-api-super-secret-key-32bytes-minimum-2025" }
        };

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings!)
            .Build();

        // Subclasse leve para injetar opções InMemory
        var options = new DbContextOptionsBuilder<DbContexto>()
            .UseInMemoryDatabase(databaseName: "test_db")
            .Options;

        return new DbContextoDeTeste(configuration, options);
    }

    private class DbContextoDeTeste : DbContexto
    {
        private readonly DbContextOptions<DbContexto> _options;
        public DbContextoDeTeste(IConfiguration configuration, DbContextOptions<DbContexto> options) : base(configuration)
        {
            _options = options;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Força o uso do provedor InMemory durante os testes
            optionsBuilder.UseInMemoryDatabase("test_db");
        }
    }


    [TestMethod]
    public void TestandoSalvarAdministrador()
    {
        // Arrange
    var context = CriarContextoDeTeste();
    // Limpa entidades usando InMemory
    context.Administradores.RemoveRange(context.Administradores);
    context.SaveChanges();

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);

        // Assert
        Assert.AreEqual(1, administradorServico.Todos(1).Count());
    }

    [TestMethod]
    public void TestandoBuscaPorId()
    {
        // Arrange
    var context = CriarContextoDeTeste();
    context.Administradores.RemoveRange(context.Administradores);
    context.SaveChanges();

        var adm = new Administrador();
        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";

        var administradorServico = new AdministradorServico(context);

        // Act
        administradorServico.Incluir(adm);
        var admDoBanco = administradorServico.BuscaPorId(adm.Id);

        // Assert
    Assert.AreEqual(adm.Id, admDoBanco?.Id);
    }
}