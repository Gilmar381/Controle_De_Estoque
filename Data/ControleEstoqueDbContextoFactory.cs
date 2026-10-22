using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Controle_De_estoqueAPI.Data
{
    public class ControleEstoqueDbContextoFactory : IDesignTimeDbContextFactory<ControleEstoqueDbContexto>
    {
        public ControleEstoqueDbContexto CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddUserSecrets<ControleEstoqueDbContexto>(optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' não encontrada. " +
                    "Configure via 'dotnet user-secrets set' (dev) ou variável de ambiente ConnectionStrings__DefaultConnection (produção).");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ControleEstoqueDbContexto>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ControleEstoqueDbContexto(optionsBuilder.Options);
        }
    }
}