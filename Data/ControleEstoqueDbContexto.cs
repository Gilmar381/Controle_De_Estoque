using Controle_De_estoqueAPI.Constantes;
using Controle_De_estoqueAPI.Models;
using Microsoft.EntityFrameworkCore;

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

            // Consultas por período
            modelBuilder.Entity<MovimentacaoEstoque>()
                .HasIndex(m => m.DataMovimentacao);

            // Nenhuma exclusão em cascata, exceto os itens de uma venda (que pertencem a ela).
            // Assim, apagar um produto/usuário nunca leva junto vendas e histórico de estoque.
            foreach (var fk in modelBuilder.Model.GetEntityTypes()
                         .SelectMany(e => e.GetForeignKeys())
                         .ToList())
            {
                var ehItemDaVenda = fk.DeclaringEntityType.ClrType == typeof(ItemVenda)
                                    && fk.PrincipalEntityType.ClrType == typeof(Venda);

                fk.DeleteBehavior = ehItemDaVenda ? DeleteBehavior.Cascade : DeleteBehavior.Restrict;
            }
        }
    }
}
