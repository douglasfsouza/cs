using Microsoft.EntityFrameworkCore.Migrations;

namespace OData2.Migrations
{
    public partial class upd : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into cidades values ('Jandira','SP')");
            migrationBuilder.Sql("insert into cidades values ('Registro','SP')");
            migrationBuilder.Sql("insert into cidades values ('Curitiba','PR')");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
