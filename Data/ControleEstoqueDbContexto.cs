using Microsoft.EntityFrameworkCore;
using Controle_De_estoqueAPI.Models;
using Controle_De_estoqueAPI.Constantes;

namespace Controle_De_estoqueAPI.Data
{
    public class ControleEstoqueDbContexto : DbContext
    {
        public ControleEstoqueDbContexto(DbContextOptions<ControleEstoqueDbContexto> options) : base(options)
        {
        }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<MovimentacaoEstoque> MovimentacaoEstoques { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Perfil> Perfis { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Perfil>().HasData(
                new Perfil { Id = 1, PerfilNome = PerfilConst.Caixa },
                new Perfil { Id = 2, PerfilNome = PerfilConst.Estoquista },
                new Perfil { Id = 3, PerfilNome = PerfilConst.Gerente }
            );
        }
    }
}
