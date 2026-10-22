using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Controle_De_estoqueAPI.Migrations
{
    /// <inheritdoc />
    public partial class NovaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Perfis_PerfisId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PerfisId",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PerfisId",
                table: "Usuarios");

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfilId",
                table: "Usuarios",
                column: "PerfilId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Perfis_PerfilId",
                table: "Usuarios",
                column: "PerfilId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Usuarios_Perfis_PerfilId",
                table: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Usuarios_PerfilId",
                table: "Usuarios");

            migrationBuilder.AddColumn<int>(
                name: "PerfisId",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_PerfisId",
                table: "Usuarios",
                column: "PerfisId");

            migrationBuilder.AddForeignKey(
                name: "FK_Usuarios_Perfis_PerfisId",
                table: "Usuarios",
                column: "PerfisId",
                principalTable: "Perfis",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
