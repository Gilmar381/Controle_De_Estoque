using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Controle_De_estoqueAPI.Migrations
{
    /// <inheritdoc />
    public partial class AjustesModelsCategoriaPerfilIndices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Usuarios_UsuarioId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_UsuarioId",
                table: "Vendas");

            migrationBuilder.RenameColumn(
                name: "EmaiFornecedor",
                table: "Fornecedores",
                newName: "EmailFornecedor");

            migrationBuilder.AddColumn<int>(
                name: "usuario",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "PerfilId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "EmailUsuario",
                table: "Usuarios",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "PerfisId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "FornecedorId",
                table: "Produtos",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "CategoriaId",
                table: "Produtos",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "Categorias",
                columns: table => new
                {
                    CategoriaID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCategoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categorias", x => x.CategoriaID);
                });

            migrationBuilder.CreateTable(
                name: "Perfis",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Perfis", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_usuario",
                table: "Vendas",
                column: "usuario");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_EmailUsuario",
                table: "Usuarios",
                column: "EmailUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios",
                column: "Login",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfisId",
                table: "Usuarios",
                column: "PerfisId");

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos",
                column: "CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "CategoriaID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Perfis_PerfisId",
                table: "Usuarios",
                column: "PerfisId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Usuarios_usuario",
                table: "Vendas",
                column: "usuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Categorias_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos");

            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Perfis_PerfisId",
                table: "Usuarios");

            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Usuarios_usuario",
                table: "Vendas");

            migrationBuilder.DropTable(
                name: "Categorias");

            migrationBuilder.DropTable(
                name: "Perfis");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_usuario",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_EmailUsuario",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_Login",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PerfisId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_CategoriaId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "usuario",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "PerfisId",
                table: "Usuarios");

            migrationBuilder.RenameColumn(
                name: "EmailFornecedor",
                table: "Fornecedores",
                newName: "EmaiFornecedor");

            migrationBuilder.AlterColumn<string>(
                name: "PerfilId",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "EmailUsuario",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "FornecedorId",
                table: "Produtos",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "CategoriaId",
                table: "Produtos",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_UsuarioId",
                table: "Vendas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Fornecedores_FornecedorId",
                table: "Produtos",
                column: "FornecedorId",
                principalTable: "Fornecedores",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Usuarios_UsuarioId",
                table: "Vendas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
