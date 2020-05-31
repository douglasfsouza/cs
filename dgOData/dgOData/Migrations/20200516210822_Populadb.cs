using Microsoft.EntityFrameworkCore.Migrations;

namespace dgOData.Migrations
{
    public partial class Populadb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Insert into Escolas(nome, estado, cidade) Values('ESAB','SP','Sao Paulo')");
            migrationBuilder.Sql("Insert into Alunos(nome,nota,escolaId) Values('Douglas',10,1)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("Delete from escolas");
            migrationBuilder.Sql("Delete from alunos");
        }
    }
}
