using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Controle_De_estoqueAPI.Migrations
{
    /// <inheritdoc />
    public partial class SeedPerfis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Usuarios_usuario",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_usuario",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "usuario",
                table: "Vendas");

            migrationBuilder.AddColumn<string>(
                name: "PerfilNome",
                table: "Perfis",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "Perfis",
                columns: new[] { "Id", "PerfilNome" },
                values: new object[,]
                {
                    { 1, "Caixa" },
                    { 2, "Estoquista" },
                    { 3, "Gerente" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_UsuarioId",
                table: "Vendas",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Usuarios_UsuarioId",
                table: "Vendas",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vendas_Usuarios_UsuarioId",
                table: "Vendas");

            migrationBuilder.DropIndex(
                name: "IX_Vendas_UsuarioId",
                table: "Vendas");

            migrationBuilder.DeleteData(
                table: "Perfis",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Perfis",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Perfis",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DropColumn(
                name: "PerfilNome",
                table: "Perfis");

            migrationBuilder.AddColumn<int>(
                name: "usuario",
                table: "Vendas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Vendas_usuario",
                table: "Vendas",
                column: "usuario");

            migrationBuilder.AddForeignKey(
                name: "FK_Vendas_Usuarios_usuario",
                table: "Vendas",
                column: "usuario",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
